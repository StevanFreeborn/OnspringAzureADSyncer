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
    try
    {
      var users = await _graphServiceClient.Users.GetAsync(
        requestConfiguration =>
        {
          requestConfiguration.QueryParameters.Count = true;
          requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
        }
      );

      var groups = await _graphServiceClient.Groups.GetAsync(
        requestConfiguration =>
        {
          requestConfiguration.QueryParameters.Count = true;
          requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
        }
      );

      if (users is null)
      {
        _logger.Debug("No users found in Azure AD");
      }

      if (groups is null)
      {
        _logger.Debug("No groups found in Azure AD");
      }

      if (users is null || groups is null)
      {
        return true;
      }

      _logger.Debug(
        "Found {Users} users and {Groups} groups in Azure AD",
        users.OdataCount,
        groups.OdataCount
      );

      return true;
    }
    catch (Exception ex)
    {
      _logger.Error("Unable to connect to Azure AD to get users and/or groups: {Exception}", ex);
      return false;
    }
  }
}