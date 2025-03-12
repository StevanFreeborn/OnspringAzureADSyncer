namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class MsGraph(GraphServiceClient graphServiceClient) : IMsGraph
{
  public async Task<DirectoryObjectCollectionResponse?> GetUserGroups(string? userId)
  {
    return await GraphServiceClient
      .Users[userId]
      .MemberOf
      .GetAsync();
  }

  public async Task<UserCollectionResponse?> GetUsersForIterator(Dictionary<int, string> usersFieldMappings)
  {
    return await GraphServiceClient
    .Users
    .GetAsync(
      config =>
      config
      .QueryParameters
      .Select = [.. usersFieldMappings.Values]
    );
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
}