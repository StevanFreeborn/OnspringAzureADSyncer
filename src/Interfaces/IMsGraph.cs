namespace OnspringAzureADSyncer.Interfaces;

public interface IMsGraph
{
  public GraphServiceClient GraphServiceClient { get; init; }
  Task<DirectoryObjectCollectionResponse?> GetUserGroups(string? userId);
  Task<GroupCollectionResponse?> GetGroups();
  Task<GroupCollectionResponse?> GetGroupsForIterator(Dictionary<int, string> groupFieldMappings);
  Task<UserCollectionResponse?> GetUsers();
  Task<UserCollectionResponse?> GetUsersForIterator(Dictionary<int, string> usersFieldMappings);
}