namespace OnspringAzureADSyncer.Interfaces;

public interface IGraphService
{
  Task<List<DirectoryObject>> GetUserGroups(User azureUser);
  Task<PageIterator<Group, GroupCollectionResponse>?> GetGroupsIterator(List<Group> groups, int pageSize);
  Task<PageIterator<User, UserCollectionResponse>?> GetUsersIterator(List<User> azureUsers, int pageSize);
  Task<bool> IsConnected();
  Task<bool> CanGetGroups();
  Task<bool> CanGetUsers();
}