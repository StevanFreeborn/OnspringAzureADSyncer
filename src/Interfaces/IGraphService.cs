namespace OnspringAzureADSyncer.Interfaces;

public interface IGraphService
{
  Task<bool> IsConnected();
}