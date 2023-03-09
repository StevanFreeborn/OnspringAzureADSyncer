namespace OnspringAzureADSyncer.Interfaces
{
  public interface IProcessor
  {
    Task SetDefaultGroupsFieldMappings();
    Task SetDefaultUsersFieldMappings();
    Task SyncGroups();
    Task SyncUsers();
    Task<bool> VerifyConnections();
  }
}