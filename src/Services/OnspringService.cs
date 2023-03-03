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

    try
    {
      _onspringClient = new OnspringClient(
        _settings.Onspring.BaseUrl,
        _settings.Onspring.ApiKey
      );
    }
    catch (Exception ex)
    {
      _logger.Fatal(
        "Unable to create Onspring client: {Exception}",
        ex
      );
      throw;
    }
    _onspringClient = new OnspringClient(
      _settings.Onspring.BaseUrl,
      _settings.Onspring.ApiKey
    );
  }

  public async Task<ResultRecord?> GetGroup(string? id)
  {
    var groupNameFieldId = _settings.GroupsFieldMappings[AzureSettings.GroupsNameKey];

    var request = new QueryRecordsRequest
    {
      AppId = _settings.Onspring.GroupsAppId,
      Filter = $"{groupNameFieldId} eq '{id}'",
    };

    var res = await ExecuteRequest(
      async () =>
        await _onspringClient.QueryRecordsAsync(request)
    );

    if (res.IsSuccessful is false)
    {
      _logger.Error(
        "Unable to get group from Onspring: {Response}",
        res
      );
      return null;
    }

    return res.Value.Items.FirstOrDefault();
  }

  public async Task<List<Field>> GetGroupFields()
  {
    var fields = new List<Field>();
    var totalPages = 1;
    var pagingRequest = new PagingRequest(1, 50);
    var currentPage = pagingRequest.PageNumber;

    do
    {
      var res = await ExecuteRequest(
        async () => await _onspringClient.GetFieldsForAppAsync(
          _settings.Onspring.GroupsAppId,
          pagingRequest
        )
      );

      if (res.IsSuccessful is true)
      {
        fields.AddRange(res.Value.Items);
        totalPages = res.Value.TotalPages;
      }
      else
      {
        _logger.Error(
          "Unable to get fields: {Response}. Current page: {CurrentPage}. Total pages: {TotalPages}.",
          res,
          currentPage,
          totalPages
        );
      }

      pagingRequest.PageNumber++;
      currentPage = pagingRequest.PageNumber;
    } while (currentPage <= totalPages);

    return fields;
  }

  public async Task<bool> IsConnected()
  {
    return await CanGetUsers() && await CanGetGroups();
  }

  public async Task<bool> CanGetUsers()
  {
    var res = await ExecuteRequest(
      async () =>
        await _onspringClient.GetAppAsync(
          _settings.Onspring.UsersAppId
        )
    );

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
    var res = await ExecuteRequest(
      async () =>
        await _onspringClient.GetAppAsync(
          _settings.Onspring.GroupsAppId
        )
    );

    if (res.IsSuccessful is false)
    {
      _logger.Error(
        "Unable to get groups from Onspring: {Response}",
        res
      );
    }

    return res.IsSuccessful;
  }

  [ExcludeFromCodeCoverage]
  private async Task<ApiResponse<T>> ExecuteRequest<T>(Func<Task<ApiResponse<T>>> func, int retryLimit = 3)
  {
    ApiResponse<T> response;
    var retry = 1;

    do
    {
      response = await func();

      if (response.IsSuccessful is true)
      {
        return response;
      }

      _logger.Error(
        "Request to Onspring API was unsuccessful. {StatusCode} - {Message}. ({Attempt} of {AttemptLimit})",
        response.StatusCode,
        response.Message,
        retry,
        retryLimit
      );

      retry++;

      if (retry > retryLimit)
      {
        break;
      }

      var wait = 1000 * retry;

      _logger.Information(
        "Waiting {Wait}s before retrying request.",
        wait
      );

      await Task.Delay(wait);

      _logger.Information(
        "Retrying request. {Attempt} of {AttemptLimit}",
        retry,
        retryLimit
      );
    } while (retry <= retryLimit);

    _logger.Error(
      "Request failed after {RetryLimit} attempts. {StatusCode} - {Message}.",
      retryLimit,
      response.StatusCode,
      response.Message
    );

    return response;
  }
}