namespace OnspringAzureADSyncer.Interfaces;

public interface IOnspringService
{
  Task<ResultRecord?> GetGroup(string? id);
  Task<List<Field>> GetGroupFields();
  Task<bool> IsConnected();
  Task<SaveRecordResponse?> CreateGroup(Group group);
  Task<SaveRecordResponse?> UpdateGroup(Group azureGroup, ResultRecord onspringGroup);
}