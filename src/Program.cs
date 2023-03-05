class Program
{
  internal async static Task<int> Main(string[] args)
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
    var root = new RootCommand(
      "An app to sync Azure AD groups and users with an Onspring instance."
    );

    root.AddOption(configFileOption);
    root.AddOption(logLevelOption);

    var host = GetHostBuilder();

    root.SetHandler(
      async (options) =>
        await Syncer.Start(host, options),
        optionsBinder
    );

    return await root.InvokeAsync(args);
  }

  [ExcludeFromCodeCoverage]
  private static IHostBuilder GetHostBuilder()
  {
    return Host
    .CreateDefaultBuilder()
    .ConfigureServices(
      (context, services) =>
      {
        services.AddSingleton<Settings>();
        services.AddSingleton<IAzureGroupDestructuringPolicy, AzureGroupDestructuringPolicy>();
        services.AddSingleton<IOnspringService, OnspringService>();
        services.AddSingleton<IGraphService, GraphService>();
        services.AddSingleton<IProcessor, Processor>();
        services.AddSingleton<ISyncer, Syncer>();
      }
    );
  }
}