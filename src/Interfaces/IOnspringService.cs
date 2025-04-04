using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Interfaces;

public interface IOnspringService
{
  Task<ResultRecord?> GetGroup(Group azureGroup);
  Task<List<Field>> GetGroupFields();
  Task<List<Field>> GetUserFields();
  Task<bool> IsConnected();
  Task<SaveRecordResponse?> CreateGroup(Group group);
  Task<SaveRecordResponse?> UpdateGroup(Group azureGroup, ResultRecord onspringGroup);
  Task<ResultRecord?> GetUser(User azureUser);
  Task<SaveRecordResponse?> CreateUser(User azureUser, Dictionary<string, int> usersGroupMappings);
  Task<SaveRecordResponse?> UpdateUser(User azureUser, ResultRecord onspringUser, Dictionary<string, int> usersGroupMappings);
  Task<SaveListItemResponse?> AddListValue(int listId, string listValueName);
}