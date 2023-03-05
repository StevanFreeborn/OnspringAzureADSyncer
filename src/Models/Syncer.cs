namespace OnspringAzureADSyncer.Models;

public class Syncer : ISyncer
{
  internal static Func<IHostBuilder, Options, Task<int>> Start = StartUp;
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

    _logger.Information("Connected successfully to Onspring and Azure AD");

    _logger.Information("Setting default field mappings");
    await _processor.SetDefaultFieldMappings();

    _logger.Information("Syncing groups");
    await _processor.SyncGroups();

    _logger.Information("Syncer finished");

    await Log.CloseAndFlushAsync();

    return 0;
  }

  public static async Task<int> StartUp(IHostBuilder builder, Options options)
  {
    try
    {
      return await builder
      .AddSerilog(options)
      .AddConfiguration(options)
      .AddOnspringClient()
      .AddGraphClient()
      .Build()
      .Services
      .GetRequiredService<ISyncer>()
      .Run();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
      return 1;
    }
  }
}