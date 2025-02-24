using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Services;

public class GraphService(
  ILogger logger,
  ISettings settings,
  IMsGraph msGraph
) : IGraphService
{
  private readonly ILogger _logger = logger;
  private readonly ISettings _settings = settings;
  private readonly IMsGraph _msGraph = msGraph;

  public async Task<List<Group>> GetUserGroups(User azureUser)
  {
    try
    {
      var groups = await _msGraph.GetUserGroups(azureUser.Id);

      if (
        groups == null ||
        groups.Value == null
      )
      {
        _logger.Warning(
          "No groups found for user {@AzureUser}",
          azureUser
        );

        return [];
      }

      return [.. groups.Value.OfType<Group>()];
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to connect to Azure AD to get groups for user {@AzureUser}: {Message}",
        azureUser,
        ex.Message
      );

      return [];
    }
  }

  public async Task<PageIterator<User, UserCollectionResponse>?> GetUsersIterator(List<User> azureUsers, int pageSize)
  {
    try
    {
      var initialUsers = await _msGraph.GetUsersForIterator(_settings.UsersFieldMappings);

      if (
        initialUsers == null ||
        initialUsers.Value == null
      )
      {
        _logger.Warning("No users found in Azure AD");
        return null;
      }

      var usersIterator = PageIterator<User, UserCollectionResponse>
      .CreatePageIterator(
        _msGraph.GraphServiceClient,
        initialUsers,
        (u) =>
        {
          azureUsers.Add(u);
          return azureUsers.Count < pageSize;
        }
      );

      return usersIterator;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to connect to Azure AD to get users: {Message}",
        ex.Message
      );

      return null;
    }
  }

  public async Task<PageIterator<Group, GroupCollectionResponse>?> GetGroupsIterator(List<Group> azureGroups, int pageSize)
  {
    try
    {
      var initialGroups = await _msGraph.GetGroupsForIterator(_settings.GroupsFieldMappings);

      if (
        initialGroups == null ||
        initialGroups.Value == null
      )
      {
        _logger.Warning("No groups found in Azure AD");
        return null;
      }


      var groupsIterator = PageIterator<Group, GroupCollectionResponse>
      .CreatePageIterator(
        _msGraph.GraphServiceClient,
        initialGroups,
        (g) =>
        {
          azureGroups.Add(g);
          return azureGroups.Count < pageSize;
        }
      );

      return groupsIterator;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to connect to Azure AD to get groups: {Message}",
        ex.Message
      );

      return null;
    }
  }

  public async Task<bool> IsConnected()
  {
    return await CanGetUsers() && await CanGetGroups();
  }

  public async Task<bool> CanGetGroups()
  {
    try
    {
      var groups = await _msGraph.GetGroups();

      return true;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to connect to Azure AD to get groups: {Message}",
        ex.Message
      );

      return false;
    }
  }

  public async Task<bool> CanGetUsers()
  {
    try
    {
      var users = await _msGraph.GetUsers();

      return true;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to connect to Azure AD to get users: {Message}",
        ex.Message
      );

      return false;
    }
  }
}