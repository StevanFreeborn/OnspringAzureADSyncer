using GroupFilter = OnspringAzureADSyncer.Models.GroupFilter;

namespace OnspringAzureADSyncer.Interfaces;

public interface IMsGraph
{
  GraphServiceClient GraphServiceClient { get; init; }
  Task<DirectoryObjectCollectionResponse?> GetUserGroups(string? userId);
  Task<GroupCollectionResponse?> GetGroups();
  Task<GroupCollectionResponse?> GetGroupsForIterator(Dictionary<int, string> groupFieldMappings, List<GroupFilter> groupFilters);
  Task<UserCollectionResponse?> GetUsers();
  Task<UserCollectionResponse?> GetUsersForIterator(Dictionary<int, string> usersFieldMappings);
}