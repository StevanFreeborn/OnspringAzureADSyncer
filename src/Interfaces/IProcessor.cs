namespace OnspringAzureADSyncer.Interfaces;

public interface IProcessor
{
  Task SyncUsers();
  Task SyncGroups();
  bool FieldMappingsAreValid();
  Task GetOnspringUserFields();
  Task GetOnspringGroupFields();
  void SetDefaultGroupsFieldMappings();
  void SetDefaultUsersFieldMappings();
  Task<bool> VerifyConnections();
  Task<(bool IsSuccessful, string ResultMessage)> HasValidGroupFilter();
}