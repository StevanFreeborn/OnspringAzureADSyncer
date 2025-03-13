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

  public async Task<PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>?> GetGroupMembersIterator(string groupId, List<User> groupMembers, int pageSize)
  {
    try
    {
      var initialGroupMembers = await _msGraph.GetGroupMembersForIterator(groupId, _settings.UsersFieldMappings);

      if (initialGroupMembers is null || initialGroupMembers.Value is null)
      {
        _logger.Warning("No group members found in Azure AD for group {GroupId}", groupId);
        return null;
      }

      var groupMembersIterator = PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>
        .CreatePageIterator(
          _msGraph.GraphServiceClient,
          initialGroupMembers,
          (u) =>
          {
            if (u is User user)
            {
              groupMembers.Add(user);
            }

            return groupMembers.Count < pageSize;
          }
        );

      return groupMembersIterator;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to connect to Azure AD to get group members for group {GroupId}: {Message}",
        groupId,
        ex.Message
      );

      return null;
    }
  }

  public async Task<List<Group>> GetUserGroups(User azureUser)
  {
    try
    {
      var groups = await _msGraph.GetUserGroups(azureUser.Id);

      if (groups is null || groups.Value is null)
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

      if (initialUsers is null || initialUsers.Value is null)
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

  public async Task<PageIterator<Group, GroupCollectionResponse>?> GetGroupsIterator(List<Group> groups, int pageSize)
  {
    try
    {
      var initialGroups = await _msGraph.GetGroupsForIterator(_settings.GroupsFieldMappings, _settings.Azure.GroupFilter);

      if (initialGroups is null || initialGroups.Value is null)
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
            groups.Add(g);
            return groups.Count < pageSize;
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
    var (isSuccessful, _) = await CanGetGroups();
    return await CanGetUsers() && isSuccessful;
  }

  public async Task<(bool IsSuccessful, string ResultMessage)> CanGetGroups(string? groupFilter = null)
  {
    try
    {
      var groups = await _msGraph.GetGroups(groupFilter);

      return (true, string.Empty);
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to connect to Azure AD to get groups: {Message}",
        ex.Message
      );

      return (false, ex.Message);
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