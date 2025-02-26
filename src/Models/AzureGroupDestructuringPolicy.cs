using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class AzureGroupDestructuringPolicy(ISettings settings) : IAzureGroupDestructuringPolicy
{
  private readonly ISettings _settings = settings;

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

    var groupFilterProperties = _settings
      .Azure
      .GroupFilters
      .Select(f => f.Property)
      .ToList();

    List<string> groupProperties = [.. groupFilterProperties, .. mappedGroupProperties];
    var distinctGroupProperties = groupProperties.Distinct();

    var props = value
      .GetType()
      .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
      .Where(p => distinctGroupProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));

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