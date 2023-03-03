namespace OnspringAzureADSyncer.Services;

public class GraphService : IGraphService
{
  private readonly ILogger _logger;
  private readonly Settings _settings;
  private readonly GraphServiceClient _graphServiceClient;

  public GraphService(ILogger logger, Settings settings)
  {
    _logger = logger;
    _settings = settings;

    try
    {
      _graphServiceClient = new GraphServiceClient(
        new ClientSecretCredential(
          _settings.Azure.TenantId,
          _settings.Azure.ClientId,
          _settings.Azure.ClientSecret,
          new TokenCredentialOptions
          {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
          }
        ),
        new[] { "https://graph.microsoft.com/.default" }
      );
    }
    catch (Exception ex)
    {
      _logger.Fatal(
        "Unable to create Graph client: {Exception}",
        ex
      );
      throw;
    }
  }

  public async Task<PageIterator<Group, GroupCollectionResponse>?> GetGroupsIterator(List<Group> groups, int pageSize)
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
          groups.Add(g);
          return groups.Count < pageSize;
        }
      );

      return groupsIterator;
    }
    catch (Exception ex)
    {
      _logger.Error(
        "Unable to connect to Azure AD to get groups: {Exception}",
        ex
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
        "Unable to connect to Azure AD to get groups: {Exception}",
        ex
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
        "Unable to connect to Azure AD to get users: {Exception}",
        ex
      );

      return false;
    }
  }
}