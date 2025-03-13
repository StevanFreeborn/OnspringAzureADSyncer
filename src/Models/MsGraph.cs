namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class MsGraph(GraphServiceClient graphServiceClient) : IMsGraph
{
  public async Task<DirectoryObjectCollectionResponse?> GetUserGroups(string? userId)
  {
    return await GraphServiceClient
      .Users[userId]
      .MemberOf
      .GetAsync(static config =>
      {
        config.QueryParameters.Count = true;
        config.Headers.Add("ConsistencyLevel", "eventual");
      });
  }

  public async Task<UserCollectionResponse?> GetUsersForIterator(Dictionary<int, string> usersFieldMappings)
  {
    return await GraphServiceClient
      .Users
      .GetAsync(config =>
      {
        config.QueryParameters.Select = [.. usersFieldMappings.Values];
        config.QueryParameters.Count = true;
        config.Headers.Add("ConsistencyLevel", "eventual");
      });
  }

  public GraphServiceClient GraphServiceClient { get; init; } = graphServiceClient;

  public async Task<GroupCollectionResponse?> GetGroupsForIterator(Dictionary<int, string> groupFieldMappings, string? groupFilter = null)
  {
    return await GraphServiceClient
      .Groups
      .GetAsync(config =>
      {
        config.QueryParameters.Select = [.. groupFieldMappings.Values];
        config.QueryParameters.Filter = groupFilter;
        config.QueryParameters.Count = true;
        config.Headers.Add("ConsistencyLevel", "eventual");
      }
    );
  }

  public async Task<GroupCollectionResponse?> GetGroups(string? groupFilter = null)
  {
    return await GraphServiceClient.Groups.GetAsync(config =>
    {
      config.QueryParameters.Filter = groupFilter;
      config.QueryParameters.Count = true;
      config.Headers.Add("ConsistencyLevel", "eventual");
    });
  }

  public async Task<UserCollectionResponse?> GetUsers()
  {
    return await GraphServiceClient.Users.GetAsync(static config =>
    {
      config.QueryParameters.Count = true;
      config.Headers.Add("ConsistencyLevel", "eventual");
    });
  }

  public Task<DirectoryObjectCollectionResponse?> GetGroupMembersForIterator(string groupId, Dictionary<int, string> usersFieldMappings)
  {
    return GraphServiceClient
      .Groups[groupId]
      .Members
      .GetAsync(config =>
      {
        config.QueryParameters.Select = [.. usersFieldMappings.Values];
        config.QueryParameters.Count = true;
        config.Headers.Add("ConsistencyLevel", "eventual");
      });
  }
}