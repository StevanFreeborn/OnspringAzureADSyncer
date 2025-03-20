using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Models;

[ExcludeFromCodeCoverage]
public class MsGraph(GraphServiceClient graphServiceClient) : IMsGraph
{
  public async Task<DirectoryObjectCollectionResponse?> GetUserGroups(string? userId, List<Group>? syncdGroups = null)
  {
    return await GraphServiceClient
      .Users[userId]
      .MemberOf
      .GetAsync(config =>
      {
        if (syncdGroups is not null)
        {
          config.QueryParameters.Filter = $"id in ({string.Join(",", syncdGroups.Select(g => $"'{g.Id}'"))})";
        }

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

  public Task<DirectoryObjectCollectionResponse?> GetGroupMembersForIterator(string groupId, List<string> userProperties)
  {
    return GraphServiceClient
      .Groups[groupId]
      .Members
      .GetAsync(config =>
      {
        config.QueryParameters.Select = [.. userProperties];
        config.QueryParameters.Count = true;
        config.Headers.Add("ConsistencyLevel", "eventual");
      });
  }
}