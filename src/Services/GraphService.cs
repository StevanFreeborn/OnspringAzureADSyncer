namespace OnspringAzureADSyncer.Services;

public class GraphService : IGraphService
{
  private readonly ILogger _logger;
  private readonly ISettings _settings;
  private readonly GraphServiceClient _graphServiceClient;

  public GraphService(
    ILogger logger,
    ISettings settings,
    GraphServiceClient graphServiceClient
  )
  {
    _logger = logger;
    _settings = settings;
    _graphServiceClient = graphServiceClient;
  }

  public async Task<PageIterator<Group, GroupCollectionResponse>?> GetGroupsIterator(List<Group> azureGroups, int pageSize)
  {
    try
    {
      var initialGroups = await _graphServiceClient
      .Groups
      .GetAsync(
        config =>
        config
        .QueryParameters
        .Select = _settings.GroupsFieldMappings.Keys.ToArray()
      );

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
        _graphServiceClient,
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
      var groups = await _graphServiceClient.Groups.GetAsync(
        config =>
        {
          config.QueryParameters.Count = true;
          config.Headers.Add("ConsistencyLevel", "eventual");
        }
      );

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
      var users = await _graphServiceClient.Users.GetAsync(
        config =>
        {
          config.QueryParameters.Count = true;
          config.Headers.Add("ConsistencyLevel", "eventual");
        }
      );

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