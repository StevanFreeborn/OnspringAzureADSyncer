namespace OnspringAzureADSyncer.Interfaces;

public interface ISettings
{
  static Dictionary<string, string> DefaultGroupsFieldMappings { get; } = new();
  AzureSettings Azure { get; init; }
  OnspringSettings Onspring { get; init; }
  Dictionary<string, int> GroupsFieldMappings { get; init; }
  Dictionary<string, int> UsersFieldMappings { get; init; }
}