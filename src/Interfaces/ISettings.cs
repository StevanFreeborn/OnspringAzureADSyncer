namespace OnspringAzureADSyncer.Interfaces;

public interface ISettings
{
  AzureSettings Azure { get; init; }
  OnspringSettings Onspring { get; init; }
  Dictionary<string, int> GroupsFieldMappings { get; init; }
  Dictionary<string, int> UsersFieldMappings { get; init; }
}