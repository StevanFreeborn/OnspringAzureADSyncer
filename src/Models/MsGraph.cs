namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class MsGraph : IMsGraph
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

  public GraphServiceClient GraphServiceClient { get; init; }

  public MsGraph(GraphServiceClient graphServiceClient)
  {
    GraphServiceClient = graphServiceClient;
  }

  public async Task<GroupCollectionResponse?> GetGroupsForIterator(
    Dictionary<int, string> groupFieldMappings
  )
  {
    return await GraphServiceClient
    .Groups
    .GetAsync(
      config =>
      config
      .QueryParameters
      .Select = groupFieldMappings.Values.ToArray()
    );
  }

  public async Task<GroupCollectionResponse?> GetGroups()
  {
    return await GraphServiceClient.Groups.GetAsync(
      config =>
      {
        config.QueryParameters.Count = true;
        config.Headers.Add("ConsistencyLevel", "eventual");
      }
    );
  }

  public async Task<UserCollectionResponse?> GetUsers()
  {
    return await GraphServiceClient.Users.GetAsync(
      config =>
      {
        config.QueryParameters.Count = true;
        config.Headers.Add("ConsistencyLevel", "eventual");
      }
    );
  }
}