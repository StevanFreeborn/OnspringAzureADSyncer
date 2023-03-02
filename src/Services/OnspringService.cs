namespace OnspringAzureADSyncer.Services;

public class OnspringService
{
  private readonly ILogger _logger;
  private readonly Settings _settings;

  private readonly OnspringClient _onspringClient;

  public OnspringService(ILogger logger, Settings settings)
  {
    _logger = logger;
    _settings = settings;

    _onspringClient = new OnspringClient(
      _settings.Onspring.BaseUrl,
      _settings.Onspring.ApiKey
    );
  }
}