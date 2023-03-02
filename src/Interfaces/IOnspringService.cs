namespace OnspringAzureADSyncer.Interfaces;

public interface IOnspringService
{
  Task<ResultRecord?> GetGroup(string? id);
  Task<bool> IsConnected();
}