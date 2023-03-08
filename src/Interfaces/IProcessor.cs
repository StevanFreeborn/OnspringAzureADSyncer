namespace OnspringAzureADSyncer.Interfaces
{
  public interface IProcessor
  {
    Task SetDefaultFieldMappings();
    Task SyncGroups();
    Task SyncUsers();
    Task<bool> VerifyConnections();
  }
}