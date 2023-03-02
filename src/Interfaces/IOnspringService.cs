namespace OnspringAzureADSyncer.Interfaces;

public interface IOnspringService
{
  Task<bool> IsConnected();
}