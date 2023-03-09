namespace OnspringAzureADSyncer.Services;

public class OnspringService : IOnspringService
{
  private readonly ILogger _logger;
  private readonly ISettings _settings;
  private readonly IOnspringClient _onspringClient;

  public OnspringService(
    ILogger logger,
    ISettings settings,
    IOnspringClient onspringClient
  )
  {
    _logger = logger;
    _settings = settings;
    _onspringClient = onspringClient;
  }


  public Task<ResultRecord?> GetUser(User azureUser)
  {
    throw new NotImplementedException();
  }

  public Task<SaveRecordResponse?> UpdateUser(User azureUser, ResultRecord onspringUser)
  {
    throw new NotImplementedException();
  }

  public Task<SaveRecordResponse?> CreateUser(User azureUser)
  {
    throw new NotImplementedException();
  }

  public async Task<List<Field>> GetUserFields()
  {
    return await GetAllFieldsForApp(
      _settings.Onspring.UsersAppId
    );
  }

  public async Task<SaveRecordResponse?> UpdateGroup(Group azureGroup, ResultRecord onspringGroup)
  {
    try
    {
      var updateRecord = BuildUpdatedOnspringGroupRecord(azureGroup, onspringGroup);

      if (updateRecord.FieldData.Count == 0)
      {
        _logger.Debug(
          "No fields for Onspring Group {@OnspringGroup} need to be updated with Azure AD Group {@AzureGroup} values",
          onspringGroup,
          azureGroup
        );

        return null;
      }

      var res = await ExecuteRequest(
        async () => await _onspringClient.SaveRecordAsync(updateRecord)
      );

      if (res.IsSuccessful is false)
      {
        _logger.Debug(
          "Unable to update group in Onspring: {@Group}. {@Response}",
          azureGroup,
          res
        );
        return null;
      }

      _logger.Debug(
        "Onspring Group {@OnspringGroup} updated using {@AzureGroup}: {@Response}",
        onspringGroup,
        azureGroup,
        res
      );

      return res.Value;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to update Onspring Group {@OnspringGroup} using Azure Group {@AzureGroup}",
        onspringGroup,
        azureGroup
      );

      return null;
    }
  }

  public async Task<SaveRecordResponse?> CreateGroup(Group azureGroup)
  {
    try
    {
      var newGroupRecord = BuildNewOnspringGroupRecord(azureGroup);

      if (newGroupRecord.FieldData.Count == 0)
      {
        _logger.Debug(
          "Unable to find any values for fields for group: {@Group}",
          azureGroup
        );
        return null;
      }

      var res = await ExecuteRequest(
        async () => await _onspringClient.SaveRecordAsync(newGroupRecord)
      );

      if (res.IsSuccessful is false)
      {
        _logger.Debug(
          "Unable to create group in Onspring: {@Group}. {@Response}",
          azureGroup,
          res
        );

        return null;
      }

      _logger.Debug(
        "Group {@Group} created in Onspring: {@Response}",
        azureGroup,
        res
      );

      return res.Value;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to create group in Onspring: {@Group}",
        azureGroup
      );

      return null;
    }
  }

  public async Task<ResultRecord?> GetGroup(string? id)
  {
    try
    {
      var groupNameFieldId = _settings.GroupsFieldMappings[AzureSettings.GroupsNameKey];

      var request = new QueryRecordsRequest
      {
        AppId = _settings.Onspring.GroupsAppId,
        Filter = $"{groupNameFieldId} eq '{id}'",
        FieldIds = _settings.GroupsFieldMappings.Values.ToList(),
        DataFormat = DataFormat.Formatted
      };

      var res = await ExecuteRequest(
        async () =>
          await _onspringClient.QueryRecordsAsync(request)
      );

      if (res.IsSuccessful is false)
      {
        _logger.Error(
          "Unable to get group from Onspring: {@Response}",
          res
        );
        return null;
      }

      return res.Value.Items.FirstOrDefault();
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to get group from Onspring: {Id}",
        id
      );

      return null;
    }
  }

  public async Task<List<Field>> GetGroupFields()
  {
    return await GetAllFieldsForApp(
      _settings.Onspring.GroupsAppId
    );
  }

  public async Task<bool> IsConnected()
  {
    return await CanGetUsers() && await CanGetGroups();
  }

  internal async Task<bool> CanGetUsers()
  {
    try
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
          "Unable to get users from Onspring: {@Response}",
          res
        );
      }

      return res.IsSuccessful;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Unable to get users from Onspring");
      return false;
    }
  }

  internal async Task<bool> CanGetGroups()
  {
    try
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
          "Unable to get groups from Onspring: {@Response}",
          res
        );
      }

      return res.IsSuccessful;
    }
    catch (Exception e)
    {
      _logger.Error(e, "Unable to get groups from Onspring");
      return false;
    }
  }

  [ExcludeFromCodeCoverage]
  private ResultRecord BuildUpdatedOnspringGroupRecord(
    Group azureGroup,
    ResultRecord onspringGroup
  )
  {
    var updateRecord = new ResultRecord
    {
      AppId = _settings.Onspring.GroupsAppId,
      RecordId = onspringGroup.RecordId
    };

    foreach (var kvp in _settings.GroupsFieldMappings)
    {
      var azureGroupValue = azureGroup
      .GetType()
      .GetProperty(kvp.Key.Capitalize())
      ?.GetValue(azureGroup);

      var onspringGroupValue = onspringGroup
      .FieldData
      .FirstOrDefault(f => f.FieldId == kvp.Value)
      ?.GetValue();

      if (
        onspringGroupValue is not null &&
        onspringGroupValue.Equals(azureGroupValue)
      )
      {
        _logger.Debug(
          "Field {FieldId} does not need to be updated. Onspring Group: {@OnspringGroup}. Azure AD Group: {@AzureGroup}",
          kvp.Value,
          onspringGroup,
          azureGroup
        );

        continue;
      }

      _logger.Debug(
        "Field {FieldId} needs to be updated. Onspring value: {CurrentValue}. Azure AD value: {NewValue}",
        kvp.Value,
        onspringGroupValue,
        azureGroupValue
      );

      var recordFieldValue = GetRecordFieldValue(kvp.Value, azureGroupValue);
      updateRecord.FieldData.Add(recordFieldValue);
    }

    return updateRecord;
  }

  [ExcludeFromCodeCoverage]
  private async Task<List<Field>> GetAllFieldsForApp(int appId)
  {
    var fields = new List<Field>();
    var totalPages = 1;
    var pagingRequest = new PagingRequest(1, 50);
    var currentPage = pagingRequest.PageNumber;

    do
    {
      try
      {
        var res = await ExecuteRequest(
          async () => await _onspringClient.GetFieldsForAppAsync(
            appId,
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
            "Unable to get fields for app {appId}: {@Response}. Current page: {CurrentPage}. Total pages: {TotalPages}.",
            appId,
            res,
            currentPage,
            totalPages
          );
        }
      }
      catch (Exception ex)
      {
        _logger.Error(
          ex,
          "Unable to get fields for app {appId}. Current page: {CurrentPage}. Total pages: {TotalPages}.",
          appId,
          currentPage,
          totalPages
        );
      }

      pagingRequest.PageNumber++;
      currentPage = pagingRequest.PageNumber;
    } while (currentPage <= totalPages);

    return fields;
  }

  [ExcludeFromCodeCoverage]
  private ResultRecord BuildNewOnspringGroupRecord(Group azureGroup)
  {
    var newGroupRecord = new ResultRecord
    {
      AppId = _settings.Onspring.GroupsAppId
    };

    foreach (var kvp in _settings.GroupsFieldMappings)
    {
      var fieldValue = azureGroup
      .GetType()
      .GetProperty(kvp.Key.Capitalize())
      ?.GetValue(azureGroup);

      if (fieldValue is null)
      {
        _logger.Debug(
          "Unable to find value for field {FieldId} for property {Property} on group: {@Group}",
          kvp.Value,
          kvp.Key,
          azureGroup
        );
        continue;
      }

      var recordFieldValue = GetRecordFieldValue(kvp.Value, fieldValue);
      newGroupRecord.FieldData.Add(recordFieldValue);
    }

    return newGroupRecord;
  }

  [ExcludeFromCodeCoverage]
  private static RecordFieldValue? GetRecordFieldValue(
    int fieldId,
    object? fieldValue
  )
  {
    return fieldValue is null
    ? null
    : fieldValue switch
    {
      string s => new StringFieldValue(fieldId, s),
      bool b => new StringFieldValue(fieldId, b.ToString()),
      int i => new IntegerFieldValue(fieldId, i),
      DateTime dt => new DateFieldValue(fieldId, dt.ToUniversalTime()),
      DateTimeOffset dto => new DateFieldValue(fieldId, dto.UtcDateTime),
      List<string> ls => new StringListFieldValue(fieldId, ls),
      List<int> li => new IntegerListFieldValue(fieldId, li),
      _ => new StringFieldValue(fieldId, JsonConvert.SerializeObject(fieldValue))
    };
  }

  [ExcludeFromCodeCoverage]
  private async Task<ApiResponse<T>> ExecuteRequest<T>(
    Func<Task<ApiResponse<T>>> func,
    int retry = 1
  )
  {
    ApiResponse<T> response;
    var retryLimit = 3;

    try
    {
      do
      {
        response = await func();

        if (response.IsSuccessful is true)
        {
          return response;
        }

        _logger.Warning(
          "Request was unsuccessful. {StatusCode} - {Message}. ({Attempt} of {AttemptLimit})",
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

        await Wait(retry);
      } while (retry <= retryLimit);
    }
    catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
    {
      _logger.Error(
        ex,
        "Request failed. ({Attempt} of {AttemptLimit})",
        retry,
        retryLimit
      );

      retry++;

      if (retry > retryLimit)
      {
        throw;
      }

      await Wait(retry);

      return await ExecuteRequest(func, retry);
    }

    _logger.Error(
      "Request failed after {RetryLimit} attempts. {StatusCode} - {Message}.",
      retryLimit,
      response.StatusCode,
      response.Message
    );

    return response;
  }

  [ExcludeFromCodeCoverage]
  private async Task Wait(int retryAttempt)
  {
    var isTesting = Environment.GetEnvironmentVariable("ENVIRONMENT") == "testing";

    var wait = 1000 * retryAttempt;

    _logger.Debug(
      "Waiting {Wait}s before retrying request.",
      wait
    );

    if (isTesting is false)
    {
      await Task.Delay(wait);
    }

    _logger.Debug(
      "Retrying request. {Attempt} of {AttemptLimit}",
      retryAttempt,
      3
    );
  }
}