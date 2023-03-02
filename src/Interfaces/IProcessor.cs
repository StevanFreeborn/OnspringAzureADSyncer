namespace OnspringAzureADSyncer.Interfaces
{
  public interface IProcessor
  {
    Task SyncGroups();
    Task<bool> VerifyConnections();
  }
}