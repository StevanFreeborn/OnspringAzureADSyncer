namespace OnspringAzureADSyncer.Interfaces
{
  public interface IProcessor
  {
    Task<bool> VerifyConnections();
  }
}