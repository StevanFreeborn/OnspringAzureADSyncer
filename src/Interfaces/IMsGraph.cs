namespace OnspringAzureADSyncer.Interfaces;

public interface IMsGraph
{
  public GraphServiceClient GraphServiceClient { get; init; }
  Task<GroupCollectionResponse?> GetGroups();
  Task<GroupCollectionResponse?> GetGroupsForIterator(Dictionary<string, int> groupFieldMappings);
  Task<UserCollectionResponse?> GetUsers();
}