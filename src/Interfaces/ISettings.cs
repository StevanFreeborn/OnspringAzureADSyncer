namespace OnspringAzureADSyncer.Interfaces;

public interface ISettings
{
  AzureSettings Azure { get; init; }
  OnspringSettings Onspring { get; init; }
  Dictionary<int, string> GroupsFieldMappings { get; init; }
  Dictionary<int, string> UsersFieldMappings { get; init; }
  List<string> GetMappedUserPropertiesAsCamelCase();
}