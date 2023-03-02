namespace OnspringAzureADSyncer.Models;

public class Syncer
{
  private readonly ILogger _logger;
  private readonly IProcessor _processor;

  public Syncer(ILogger logger, IProcessor processor)
  {
    _logger = logger;
    _processor = processor;
  }

  public async Task<int> Run()
  {
    _logger.Information("Starting syncer");

    if (await _processor.VerifyConnections() is false)
    {
      _logger.Fatal("Unable to connect to Onspring and/or Azure AD");
      return 2;
    }

    await _processor.SyncGroups();

    _logger.Information("Syncer finished");

    await Log.CloseAndFlushAsync();

    return 0;
  }

  public static async Task<int> StartUp(Options options)
  {
    try
    {
      return await Host
      .CreateDefaultBuilder()
      .UseSerilog(
        (context, config) =>
          config
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .WriteTo.File(
            new CompactJsonFormatter(),
            Path.Combine(
              AppContext.BaseDirectory,
              $"{DateTime.Now:yyyy_MM_dd_HHmmss}_output",
              "log.json"
            )
          )
          .WriteTo.Console(
            restrictedToMinimumLevel: options.LogLevel,
            theme: AnsiConsoleTheme.Code
          )
      )
      .ConfigureAppConfiguration(
        (context, config) =>
          config
          .AddJsonFile(
            options.ConfigFile!,
            optional: false,
            reloadOnChange: true
          )
          .AddEnvironmentVariables()
      )
      .ConfigureServices(
        (context, services) =>
        {
          services.AddSingleton<Settings>();
          services.AddSingleton<IOnspringService, OnspringService>();
          services.AddSingleton<IGraphService, GraphService>();
          services.AddSingleton<IProcessor, Processor>();
          services.AddSingleton<Syncer>();
        }
      )
      .Build()
      .Services
      .GetRequiredService<Syncer>()
      .Run();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
      return 1;
    }
  }
}