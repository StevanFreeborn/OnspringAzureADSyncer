namespace OnspringAzureADSyncer.Models
{
  public class AppOptionsBinder : BinderBase<AppOptions>
  {
    private readonly Option<FileInfo> _configFileOption;
    private readonly Option<LogEventLevel> _logLevelOption;

    public AppOptionsBinder(
      Option<FileInfo> configFileOption,
      Option<LogEventLevel> logLevelOption
    )
    {
      _configFileOption = configFileOption;
      _logLevelOption = logLevelOption;
    }

    protected override AppOptions GetBoundValue(BindingContext bindingContext)
    {
      return new AppOptions
      {
        ConfigFileOption = bindingContext.ParseResult.GetValueForOption(_configFileOption),
        LogLevelOption = bindingContext.ParseResult.GetValueForOption(_logLevelOption)
      };
    }
  }
}