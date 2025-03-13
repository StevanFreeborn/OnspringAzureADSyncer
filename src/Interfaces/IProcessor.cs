using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Interfaces;

public interface IProcessor
{
  Task SyncGroupMembers(Group azureGroup);
  Task SyncUsers();
  Task<List<Group>> SyncGroups();
  bool FieldMappingsAreValid();
  Task GetOnspringUserFields();
  Task GetOnspringGroupFields();
  void SetDefaultGroupsFieldMappings();
  void SetDefaultUsersFieldMappings();
  Task<bool> VerifyConnections();
  Task<(bool IsSuccessful, string ResultMessage)> HasValidGroupFilter();
}