namespace OnspringAzureADSyncer.Extensions
{
  public static class SyncerExtensions
  {
    public static RootCommand BuildRootCommand(this Syncer syncer)
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