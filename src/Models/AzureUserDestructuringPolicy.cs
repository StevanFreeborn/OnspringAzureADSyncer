namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class AzureUserDestructuringPolicy(ISettings settings) : IAzureUserDestructuringPolicy
{
  private readonly ISettings _settings = settings;

  public bool TryDestructure(
    object value,
    ILogEventPropertyValueFactory propertyValueFactory,
    [NotNullWhen(true)] out LogEventPropertyValue? result
  )
  {
    if (value is not User user)
    {
      result = null;
      return false;
    }

    var mappedUserProperties = _settings
      .UsersFieldMappings
      .Values
      .ToList();

    var props = value
      .GetType()
      .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
      .Where(p => mappedUserProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));

    var logEventProperties = new List<LogEventProperty>();

    foreach (var prop in props)
    {
      var propValue = prop.GetValue(value);
      var logEventPropertyValue = propertyValueFactory.CreatePropertyValue(propValue, true);
      logEventProperties.Add(new LogEventProperty(prop.Name, logEventPropertyValue));
    }

    result = new StructureValue(logEventProperties);
    return true;
  }
}