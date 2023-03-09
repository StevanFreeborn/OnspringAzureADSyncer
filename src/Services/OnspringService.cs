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

  public async Task<SaveRecordResponse?> UpdateUser(
    User azureUser,
    ResultRecord onspringUser,
    Dictionary<string, int> usersGroupMappings
  )
  {
    try
    {
      var updateRecord = BuildUpdatedOnspringUserRecord(
        azureUser,
        onspringUser,
        usersGroupMappings
      );

      if (updateRecord.FieldData.Count == 0)
      {
        _logger.Debug(
          "No fields for Onspring User {@OnspringUser} need to be updated with Azure AD User {@AzureUser} values",
          onspringUser,
          azureUser
        );

        return null;
      }

      var res = await ExecuteRequest(
        async () => await _onspringClient.SaveRecordAsync(updateRecord)
      );

      if (res.IsSuccessful is false)
      {
        _logger.Debug(
          "Unable to update Onspring User {@OnspringUser} with Azure User {@AzureUser}. {@Response}",
          onspringUser,
          azureUser,
          res
        );
        return null;
      }

      _logger.Debug(
        "Onspring User {@OnspringUser} updated using Azure User {@AzureUser}: {@Response}",
        onspringUser,
        azureUser,
        res
      );

      return res.Value;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to update Onspring User {@OnspringUser} using Azure User {@AzureUser}",
        onspringUser,
        azureUser
      );

      return null;
    }
  }

  public async Task<SaveRecordResponse?> CreateUser(
    User azureUser,
    Dictionary<string, int> usersGroupMappings
  )
  {
    try
    {
      var newUserRecord = BuildNewOnspringUserRecord(
        azureUser,
        usersGroupMappings
      );

      if (newUserRecord.FieldData.Count == 0)
      {
        _logger.Debug(
          "Unable to find any values for fields for user: {@AzureUser}",
          azureUser
        );
        return null;
      }

      var res = await ExecuteRequest(
        async () => await _onspringClient.SaveRecordAsync(newUserRecord)
      );

      if (res.IsSuccessful is false)
      {
        _logger.Debug(
          "Unable to create user in Onspring: {@AzureUser}. {@Response}",
          azureUser,
          res
        );

        return null;
      }

      _logger.Debug(
        "User {@AzureUser} created in Onspring: {@Response}",
        azureUser,
        res
      );

      return res.Value;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to create user in Onspring: {@AzureUser}",
        azureUser
      );

      return null;
    }
  }

  public async Task<ResultRecord?> GetUser(User azureUser)
  {
    try
    {
      var userUsernameFieldId = _settings.Onspring.UsersUsernameFieldId;

      var azurePropertyMappedToUserName = _settings
      .UsersFieldMappings
      .FirstOrDefault(
        x => x.Value == userUsernameFieldId
      ).Key;

      var lookupValue = azureUser
      .GetType()
      .GetProperty(
        azurePropertyMappedToUserName.Capitalize()
      )
      ?.GetValue(azureUser);

      var request = new QueryRecordsRequest
      {
        AppId = _settings.Onspring.UsersAppId,
        Filter = $"{userUsernameFieldId} eq '{lookupValue}'",
        FieldIds = _settings.UsersFieldMappings.Values.ToList(),
        DataFormat = DataFormat.Formatted
      };

      var res = await ExecuteRequest(
        async () =>
          await _onspringClient.QueryRecordsAsync(request)
      );

      if (res.IsSuccessful is false)
      {
        _logger.Error(
          "Unable to get user from Onspring: {@Response}",
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
        "Unable to get user from Onspring: {@AzureUser}",
        azureUser
      );

      return null;
    }
  }


  public async Task<List<Field>> GetUserFields()
  {
    return await GetAllFieldsForApp(
      _settings.Onspring.UsersAppId
    );
  }

  public async Task<SaveRecordResponse?> UpdateGroup(
    Group azureGroup,
    ResultRecord onspringGroup
  )
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
          "Unable to Onspring Group {@OnspringGroup} with Azure Group {@AzureGroup}. {@Response}",
          onspringGroup,
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
        "Group {@AzureGroup} created in Onspring: {@Response}",
        azureGroup,
        res
      );

      return res.Value;
    }
    catch (Exception ex)
    {
      _logger.Error(
        ex,
        "Unable to create group in Onspring: {@AzureGroup}",
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
      var requestFieldIds = _settings.GroupsFieldMappings.Values.ToList();

      var request = new QueryRecordsRequest
      {
        AppId = _settings.Onspring.GroupsAppId,
        Filter = $"{groupNameFieldId} eq '{id}'",
        FieldIds = requestFieldIds,
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

  internal async Task<List<Field>> GetAllFieldsForApp(int appId)
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

  internal ResultRecord BuildUpdatedOnspringUserRecord(
    User azureUser,
    ResultRecord onspringUser,
    Dictionary<string, int> usersGroupMappings
  )
  {
    var updatedOnspringUser = BuildUpdatedRecord(
      azureUser,
      onspringUser,
      _settings.UsersFieldMappings
    );

    var groupsFieldId = _settings.UsersFieldMappings[AzureSettings.UsersGroupsKey];

    updatedOnspringUser
    .FieldData
    .Add(
      new IntegerListFieldValue(
        groupsFieldId,
        usersGroupMappings.Values.ToList()
      )
    );

    return SetUsersStatus(
      updatedOnspringUser,
      usersGroupMappings.Keys.ToList()
    );
  }

  internal ResultRecord BuildNewOnspringUserRecord(
    User azureUser,
    Dictionary<string, int> usersGroupMappings
  )
  {
    var newOnspringUser = BuildNewRecord(
      azureUser,
      _settings.UsersFieldMappings,
      _settings.Onspring.UsersAppId
    );

    var groupsFieldId = _settings.UsersFieldMappings[AzureSettings.UsersGroupsKey];

    newOnspringUser
    .FieldData
    .Add(
      new IntegerListFieldValue(
        groupsFieldId,
        usersGroupMappings.Values.ToList()
      )
    );

    return SetUsersStatus(
      newOnspringUser,
      usersGroupMappings.Keys.ToList()
    );
  }

  internal ResultRecord SetUsersStatus(
    ResultRecord onspringUser,
    List<string> usersGroupIds
  )
  {
    var statusFieldId = _settings.UsersFieldMappings[AzureSettings.UsersStatusKey];

    var existingStatusValue = onspringUser
    .FieldData
    .FirstOrDefault(
      f => f.FieldId == statusFieldId
    );

    onspringUser
    .FieldData
    .Remove(
      existingStatusValue
    );

    if (
      existingStatusValue.GetValue() is "True" &&
      usersGroupIds.Any(
        g => _settings.Azure.OnspringActiveGroups.Contains(g)
      )
    )
    {
      var activeStatusValue = new StringFieldValue(
        statusFieldId,
        _settings.Onspring.UserActiveStatusListValue.ToString()
      );

      onspringUser
      .FieldData
      .Add(activeStatusValue);

      return onspringUser;
    }

    var inactiveStatusValue = new StringFieldValue(
      statusFieldId,
      _settings.Onspring.UserInactiveStatusListValue.ToString()
    );


    onspringUser
    .FieldData
    .Add(inactiveStatusValue);

    return onspringUser;
  }

  internal ResultRecord BuildUpdatedOnspringGroupRecord(
    Group azureGroup,
    ResultRecord onspringGroup
  )
  {
    return BuildUpdatedRecord(
      azureGroup,
      onspringGroup,
      _settings.GroupsFieldMappings
    );
  }

  internal ResultRecord BuildNewOnspringGroupRecord(Group azureGroup)
  {
    return BuildNewRecord(
      azureGroup,
      _settings.GroupsFieldMappings,
      _settings.Onspring.GroupsAppId
    );
  }

  internal ResultRecord BuildUpdatedRecord(
    object azureObject,
    ResultRecord onspringRecord,
    Dictionary<string, int> fieldMappings
  )
  {
    var updateRecord = new ResultRecord
    {
      AppId = onspringRecord.AppId,
      RecordId = onspringRecord.RecordId
    };

    foreach (var kvp in fieldMappings)
    {
      if (kvp.Key == AzureSettings.UsersGroupsKey)
      {
        continue;
      }

      if (kvp.Key == AzureSettings.UsersStatusKey)
      {
        continue;
      }

      var azureObjectValue = azureObject
      .GetType()
      .GetProperty(kvp.Key.Capitalize())
      ?.GetValue(azureObject);

      var onspringRecordValue = onspringRecord
      .FieldData
      .FirstOrDefault(f => f.FieldId == kvp.Value)
      ?.GetValue();

      if (
        onspringRecordValue is not null &&
        onspringRecordValue.Equals(azureObjectValue)
      )
      {
        _logger.Debug(
          "Field {FieldId} does not need to be updated. Onspring Record: {@OnspringRecord}. Azure AD Object: {@AzureObject}",
          kvp.Value,
          onspringRecord,
          azureObject
        );

        continue;
      }

      _logger.Debug(
        "Field {FieldId} needs to be updated. Onspring value: {CurrentValue}. Azure AD value: {NewValue}",
        kvp.Value,
        onspringRecordValue,
        azureObjectValue
      );

      var recordFieldValue = GetRecordFieldValue(kvp.Value, azureObjectValue);
      updateRecord.FieldData.Add(recordFieldValue);
    }

    return updateRecord;
  }

  internal ResultRecord BuildNewRecord(
    object azureObject,
    Dictionary<string, int> fieldMappings,
    int appId
  )
  {
    var newRecord = new ResultRecord
    {
      AppId = appId
    };

    foreach (var kvp in fieldMappings)
    {
      var fieldValue = azureObject
      .GetType()
      .GetProperty(kvp.Key.Capitalize())
      ?.GetValue(azureObject);

      if (fieldValue is null)
      {
        _logger.Debug(
          "Unable to find value for field {FieldId} for property {Property} on: {@AzureObject}",
          kvp.Value,
          kvp.Key,
          azureObject
        );
        continue;
      }

      var recordFieldValue = GetRecordFieldValue(kvp.Value, fieldValue);
      newRecord.FieldData.Add(recordFieldValue);
    }

    return newRecord;
  }

  internal static RecordFieldValue? GetRecordFieldValue(
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