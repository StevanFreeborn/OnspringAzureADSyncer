namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class MsGraph : IMsGraph
{
  public async Task<UserCollectionResponse?> GetUsersForIterator(Dictionary<string, int> usersFieldMappings)
  {
    return await GraphServiceClient
    .Users
    .GetAsync(
      config =>
      config
      .QueryParameters
      .Select = usersFieldMappings.Keys.ToArray()
    );
  }

  public GraphServiceClient GraphServiceClient { get; init; }

  public MsGraph(GraphServiceClient graphServiceClient)
  {
    GraphServiceClient = graphServiceClient;
  }

  public async Task<GroupCollectionResponse?> GetGroupsForIterator(
    Dictionary<string, int> groupFieldMappings
  )
  {
    return await GraphServiceClient
    .Groups
    .GetAsync(
      config =>
      config
      .QueryParameters
      .Select = groupFieldMappings.Keys.ToArray()
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