[ExcludeFromCodeCoverage]
class Program
{
  async static Task<int> Main(string[] args)
  {
    return await BuildCommandLine()
    .UseDefaults()
    .Build()
    .InvokeAsync(args);
  }

  static CommandLineBuilder BuildCommandLine()
  {
    var configFileOption = new Option<FileInfo>(
      aliases: new string[] { "--config", "-c" },
      description: "The path to the file that specifies configuration for the syncer."
    )
    {
      IsRequired = true,
    };

    configFileOption.AddValidator(
      result =>
      {
        var value = result.GetValueOrDefault<FileInfo>();

        if (
          value is not null &&
          value.Exists is false
        )
        {
          result.ErrorMessage = $"The config file '{value.FullName}' does not exist.";
        }
      }
    );

    var logLevelOption = new Option<LogEventLevel>(
      aliases: new string[] { "--log-level", "-l" },
      description: "The level of logging to use.",
      getDefaultValue: () => LogEventLevel.Information
    );

    var rootCommand = new RootCommand(
      "An app to sync Azure AD groups and users with an Onspring instance."
    )
    {
      configFileOption,
      logLevelOption,
    };

    var modelBinder = new ModelBinder<AppOptions>();

    modelBinder.BindMemberFromValue(
      opt => opt.ConfigFile, configFileOption
    );

    modelBinder.BindMemberFromValue(
      opt => opt.LogLevel, logLevelOption
    );

    rootCommand.SetHandler(
      context => StartUp(context, modelBinder)
    );

    return new CommandLineBuilder(rootCommand);
  }

  static async Task<int> StartUp(InvocationContext context, ModelBinder<AppOptions> modelBinder)
  {
    try
    {
      return await Host
      .CreateDefaultBuilder()
      .ConfigureAppConfiguration(
        (hostContext, config) =>
          config.AddEnvironmentVariables()
      )
      .ConfigureServices(
        services =>
        {
          services.AddSingleton(modelBinder);
          services.AddSingleton(context.BindingContext);
          services.AddOptions<AppOptions>().BindCommandLine();
          services.AddSingleton<ISettings, Settings>();
          services.AddSingleton<IAzureGroupDestructuringPolicy, AzureGroupDestructuringPolicy>();
          services.AddSingleton<IMsGraph, MsGraph>();
          services.AddSingleton<IOnspringService, OnspringService>();
          services.AddSingleton<IGraphService, GraphService>();
          services.AddSingleton<IProcessor, Processor>();
          services.AddSingleton<ISyncer, Syncer>();
        }
      )
      .AddSerilog()
      .AddOnspringClient()
      .AddGraphClient()
      .Build()
      .Services
      .GetRequiredService<ISyncer>()
      .Run();
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("A fatal error occurred while running the syncer:");
      Console.WriteLine(ex.Message);
      return 1;
    }
  }
}