using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Interfaces;

public interface IGraphService
{
  Task<PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>?> GetGroupMembersIterator(string groupId, List<User> groupMembers, int pageSize);
  Task<List<Group>> GetUserGroups(User azureUser);
  Task<PageIterator<Group, GroupCollectionResponse>?> GetGroupsIterator(List<Group> groups, int pageSize);
  Task<PageIterator<User, UserCollectionResponse>?> GetUsersIterator(List<User> azureUsers, int pageSize);
  Task<bool> IsConnected();
  Task<(bool IsSuccessful, string ResultMessage)> CanGetGroups(string? groupFilter = null);
  Task<bool> CanGetUsers();
}