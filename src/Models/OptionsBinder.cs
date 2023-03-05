namespace OnspringAzureADSyncer.Models
{
  public class OptionsBinder : BinderBase<Options>
  {
    private readonly Option<string> _configFileOption;
    private readonly Option<LogEventLevel> _logLevelOption;

    public OptionsBinder(
      Option<string> configFileOption,
      Option<LogEventLevel> logLevelOption
    )
    {
      _configFileOption = configFileOption;
      _logLevelOption = logLevelOption;
    }

    protected override Options GetBoundValue(BindingContext bindingContext)
    {
      return new Options
      {
        ConfigFile = bindingContext.ParseResult.GetValueForOption(_configFileOption),
        LogLevel = bindingContext.ParseResult.GetValueForOption(_logLevelOption)
      };
    }
  }
}