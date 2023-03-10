namespace OnspringAzureADSyncer.Interfaces
{
  public interface IProcessor
  {
    Task SyncUsers();
    Task SyncGroups();
    bool CustomFieldMappingsAreValid();
    Task GetOnspringUserFields();
    Task GetOnspringGroupFields();
    void SetDefaultGroupsFieldMappings();
    void SetDefaultUsersFieldMappings();
    Task<bool> VerifyConnections();
  }
}