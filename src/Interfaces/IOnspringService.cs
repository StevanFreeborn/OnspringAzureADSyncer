namespace OnspringAzureADSyncer.Interfaces;

public interface IOnspringService
{
  Task<ResultRecord?> GetGroup(string? id);
  Task<List<Field>> GetGroupFields();
  Task<bool> IsConnected();
}