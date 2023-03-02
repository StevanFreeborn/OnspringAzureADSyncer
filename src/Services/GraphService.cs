namespace OnspringAzureADSyncer.Services;

public class GraphService
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
        _settings.AzureAD.TenantId,
        _settings.AzureAD.ClientId,
        _settings.AzureAD.ClientSecret
      )
    );

  }
}