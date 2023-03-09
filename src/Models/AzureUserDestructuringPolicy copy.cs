namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class AzureUserDestructuringPolicy : IAzureUserDestructuringPolicy
{
  private readonly ISettings _settings;

  public AzureUserDestructuringPolicy(ISettings settings)
  {
    _settings = settings;
  }

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
    .Keys
    .Select(p => p.Capitalize())
    .ToList();

    var props = value
    .GetType()
    .GetProperties()
    .Where(
      p => mappedUserProperties.Contains(p.Name) &&
      p.GetIndexParameters().Any() == false
    );

    var logEventProperties = new List<LogEventProperty>();

    foreach (var prop in props)
    {
      var propValue = prop.GetValue(value);

      if (propValue is null)
      {
        continue;
      }

      var logEventPropertyValue = propertyValueFactory.CreatePropertyValue(propValue, true);
      logEventProperties.Add(new LogEventProperty(prop.Name, logEventPropertyValue));
    }

    result = new StructureValue(logEventProperties);
    return true;
  }
}