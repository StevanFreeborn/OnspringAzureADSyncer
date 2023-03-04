namespace OnspringAzureADSyncer.Models;

public class AzureGroupDestructuringPolicy : IDestructuringPolicy
{
  private readonly Settings _settings;

  public AzureGroupDestructuringPolicy(Settings settings)
  {
    _settings = settings;
  }

  public bool TryDestructure(
    object value,
    ILogEventPropertyValueFactory propertyValueFactory,
    [NotNullWhen(true)] out LogEventPropertyValue? result
  )
  {
    if (value is not Group group)
    {
      result = null;
      return false;
    }

    var mappedGroupProperties = _settings
    .GroupsFieldMappings
    .Keys
    .Select(p => p.Capitalize())
    .ToList();

    var props = value
    .GetType()
    .GetProperties()
    .Where(
      p => mappedGroupProperties.Contains(p.Name) &&
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