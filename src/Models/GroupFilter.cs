using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Models;

public class GroupFilter
{
  public string Property { get; init; } = string.Empty;
  public string Pattern { get; init; } = string.Empty;

  public bool IsValid()
  {
    var prop = typeof(Group).GetProperty(Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

    if (prop is null)
    {
      return false;
    }

    if (prop.PropertyType != typeof(string))
    {
      return false;
    }

    if (string.IsNullOrWhiteSpace(Pattern))
    {
      return false;
    }

    try
    {
      var regex = new Regex(Pattern);
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }

  public bool IsMatch(Group group)
  {
    var prop = group.GetType().GetProperty(Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

    if (prop is null)
    {
      return false;
    }

    var value = prop.GetValue(group)?.ToString();

    if (value is null)
    {
      return false;
    }

    try
    {
      var timeout = TimeSpan.FromSeconds(5);
      return Regex.IsMatch(value, Pattern, RegexOptions.None, timeout);
    }
    catch (Exception)
    {
      return false;
    }
  }
}