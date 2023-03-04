namespace OnspringAzureADSyncer.Services;

class OnspringService : IOnspringService
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
        ex,
        "Unable to create Onspring client: {Message}",
        ex.Message
      );
      throw;
    }
    _onspringClient = new OnspringClient(
      _settings.Onspring.BaseUrl,
      _settings.Onspring.ApiKey
    );
  }

  public async Task<SaveRecordResponse?> UpdateGroup(Group azureGroup, ResultRecord onspringGroup)
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

  public async Task<SaveRecordResponse?> CreateGroup(Group azureGroup)
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

  public async Task<ResultRecord?> GetGroup(string? id)
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
          "Unable to get fields: {@Response}. Current page: {CurrentPage}. Total pages: {TotalPages}.",
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
        "Unable to get users from Onspring: {@Response}",
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
        "Unable to get groups from Onspring: {@Response}",
        res
      );
    }

    return res.IsSuccessful;
  }

  private static RecordFieldValue? GetRecordFieldValue(int fieldId, object? fieldValue)
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
        "Request to Onspring API was unsuccessful: {@Response}. ({Attempt} of {AttemptLimit})",
        response,
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
      "Request failed after {RetryLimit} attempts: {@Response}",
      retryLimit,
      response
    );

    return response;
  }
}