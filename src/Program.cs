class Program
{
  [ExcludeFromCodeCoverage]
  internal async static Task<int> Main(string[] args)
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

    var optionsBinder = new AppOptionsBinder(configFileOption, logLevelOption);

    var rootCommand = new RootCommand(
      "An app to sync Azure AD groups and users with an Onspring instance."
    )
    {
      configFileOption,
      logLevelOption,
    };

    rootCommand.SetHandler(
      Syncer.StartUp,
      optionsBinder
    );

    return await rootCommand.InvokeAsync(args);
  }
}