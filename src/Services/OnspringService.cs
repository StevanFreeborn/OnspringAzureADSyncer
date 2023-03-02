namespace OnspringAzureADSyncer.Services;

public class OnspringService : IOnspringService
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

  public async Task<bool> IsConnected()
  {
    return await CanGetUsers() && await CanGetGroups();
  }

  public async Task<bool> CanGetUsers()
  {
    var res = await _onspringClient.GetAppAsync(_settings.Onspring.UsersAppId);

    if (res.IsSuccessful is false)
    {
      _logger.Error(
        "Unable to get users from Onspring: {Response}",
        res
      );
    }

    return res.IsSuccessful;
  }

  public async Task<bool> CanGetGroups()
  {
    var res = await _onspringClient.GetAppAsync(_settings.Onspring.GroupsAppId);

    if (res.IsSuccessful is false)
    {
      _logger.Error(
        "Unable to get groups from Onspring: {Response}",
        res
      );
    }

    return res.IsSuccessful;
  }
}