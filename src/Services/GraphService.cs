namespace OnspringAzureADSyncer.Services;

public class GraphService : IGraphService
{
  private readonly ILogger _logger;
  private readonly ISettings _settings;
  private readonly IMsGraph _msGraph;

  public GraphService(
    ILogger logger,
    ISettings settings,
    IMsGraph msGraph
  )
  {
    _logger = logger;
    _settings = settings;
    _msGraph = msGraph;
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
        _logger.Debug("No groups found in Azure AD");
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