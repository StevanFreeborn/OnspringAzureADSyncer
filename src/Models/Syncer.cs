namespace OnspringAzureADSyncer.Models
{
  public class Syncer
  {
    private readonly ILogger _logger;

    private readonly Settings _settings;

    public Syncer(ILogger logger, Settings settings)
    {
      _logger = logger;
      _settings = settings;
    }

    public async Task<int> Run()
    {
      _logger.Information("Starting syncer");
      _logger.Information("Settings: {@Settings}", _settings);
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
}