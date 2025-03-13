namespace OnspringAzureADSyncer.Interfaces;

public interface IMsGraph
{
  GraphServiceClient GraphServiceClient { get; init; }
  Task<DirectoryObjectCollectionResponse?> GetUserGroups(string? userId);
  Task<GroupCollectionResponse?> GetGroups(string? groupFilter = null);
  Task<GroupCollectionResponse?> GetGroupsForIterator(Dictionary<int, string> groupFieldMappings, string? groupFilter = null);
  Task<UserCollectionResponse?> GetUsers();
  Task<UserCollectionResponse?> GetUsersForIterator(Dictionary<int, string> usersFieldMappings);
  Task<DirectoryObjectCollectionResponse?> GetGroupMembersForIterator(string groupId, Dictionary<int, string> usersFieldMappings);
}