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

    public static async Task<int> Start(Options options)
    {
      return await Host
      .CreateDefaultBuilder()
      .UseSerilog(
        (context, config) =>
          config
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .WriteTo.File(new CompactJsonFormatter(), "log.json")
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

    public static RootCommand BuildCommand()
    {
      var configFileOption = new Option<string>(
        aliases: new string[] { "--config", "-c" },
        description: "The path to the file that specifies configuration for the syncer."
      )
      {
        IsRequired = true,
      };

      var logLevelOption = new Option<LogEventLevel>(
        aliases: new string[] { "--log-level", "-l" },
        description: "The level of logging to use.",
        getDefaultValue: () => LogEventLevel.Information
      );

      var optionsBinder = new OptionsBinder(configFileOption, logLevelOption);
      var root = new RootCommand("An app to sync Azure AD groups and users with an Onspring instance.");

      root.AddOption(configFileOption);
      root.AddOption(logLevelOption);

      root.SetHandler(
        Syncer.Start,
        optionsBinder
      );

      return root;
    }
  }
}