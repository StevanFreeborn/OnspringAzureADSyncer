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

      if (groups == null)
      {
        _logger.Debug("No groups found in Azure AD");
      }
      else
      {
        _logger.Debug(
          "Found {GroupsCount} groups in Azure AD",
          groups.OdataCount
        );
      }

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

      if (users == null)
      {
        _logger.Debug("No users found in Azure AD");
      }
      else
      {
        _logger.Debug(
          "Found {UsersCount} users in Azure AD",
          users.OdataCount
        );
      }

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