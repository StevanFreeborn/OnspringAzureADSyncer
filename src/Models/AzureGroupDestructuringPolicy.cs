namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class AzureGroupDestructuringPolicy : IAzureGroupDestructuringPolicy
{
  private readonly ISettings _settings;

  public AzureGroupDestructuringPolicy(ISettings settings)
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
      .Values
      .ToList();

    var props = value
      .GetType()
      .GetProperties(
        BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public
      )
      .Where(
        p => mappedGroupProperties.Contains(p.Name) &&
          p.GetIndexParameters().Length != 0
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