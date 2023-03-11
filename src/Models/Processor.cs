namespace OnspringAzureADSyncer.Models;

public class Processor : IProcessor
{
  private readonly ILogger _logger;
  private readonly ISettings _settings;
  private readonly IOnspringService _onspringService;
  private readonly IGraphService _graphService;

  public Processor(
    ILogger logger,
    ISettings settings,
    IOnspringService onspringService,
    IGraphService graphService
  )
  {
    _logger = logger;
    _settings = settings;
    _onspringService = onspringService;
    _graphService = graphService;
  }

  public async Task SyncUsers()
  {
    var azureUsers = new List<User>();
    var pageSize = 50;
    var userIterator = await _graphService.GetUsersIterator(azureUsers, pageSize);

    if (userIterator is null)
    {
      _logger.Warning("No users found in Azure AD");
      return;
    }

    var pageNumberProcessing = 0;

    do
    {
      if (userIterator.State == PagingState.Paused)
      {
        await userIterator.ResumeAsync();
      }

      if (userIterator.State == PagingState.NotStarted)
      {
        await userIterator.IterateAsync();
      }

      if (azureUsers.Count == 0)
      {
        continue;
      }

      pageNumberProcessing++;

      var options = new ProgressBarOptions
      {
        ForegroundColor = ConsoleColor.DarkBlue,
        ProgressCharacter = '─',
        ShowEstimatedDuration = false,
      };

      using (
        var progressBar = new ProgressBar(
          azureUsers.Count,
          $"Starting processing Azure Users: page {pageNumberProcessing}",
          options
        )
      )
      {
        await Parallel.ForEachAsync(azureUsers, async (azureUser, token) =>
        {
          await SyncUser(azureUser);
          progressBar.Tick($"Processing Azure User: {azureUser.Id}");
        });

        progressBar.Message = $"Finished processing Azure Users: page {pageNumberProcessing}";
      }

      // clear the list before
      // the next iteration
      // to avoid processing a user
      // multiple times
      azureUsers.Clear();

    } while (userIterator.State != PagingState.Complete);
  }

  public async Task SyncGroups()
  {
    var azureGroups = new List<Group>();
    var pageSize = 50;
    var groupIterator = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    if (groupIterator is null)
    {
      _logger.Warning("No groups found in Azure AD");
      return;
    }

    var pageNumberProcessing = 0;

    do
    {
      if (groupIterator.State == PagingState.Paused)
      {
        await groupIterator.ResumeAsync();
      }

      if (groupIterator.State == PagingState.NotStarted)
      {
        await groupIterator.IterateAsync();
      }

      if (azureGroups.Count == 0)
      {
        continue;
      }

      pageNumberProcessing++;

      var options = new ProgressBarOptions
      {
        ForegroundColor = ConsoleColor.DarkBlue,
        ProgressCharacter = '─',
        ShowEstimatedDuration = false,
      };

      using (
        var progressBar = new ProgressBar(
          azureGroups.Count,
          $"Starting processing Azure Groups: page {pageNumberProcessing}",
          options
        )
      )
      {
        await Parallel.ForEachAsync(azureGroups, async (azureGroup, token) =>
        {
          await SyncGroup(azureGroup);
          progressBar.Tick($"Processing Azure Group: {azureGroup.Id}");
        });

        progressBar.Message = $"Finished processing Azure Groups: page {pageNumberProcessing}";
      }

      // clear the list before
      // the next iteration
      // to avoid processing a group
      // multiple times
      azureGroups.Clear();

    } while (groupIterator.State != PagingState.Complete);
  }

  public bool FieldMappingsAreValid()
  {
    return HasValidAzureProperties() &&
    HasValidOnspringFields() &&
    HasRequiredOnspringFields() &&
    HasValidFieldTypeToPropertyTypeMappings();
  }

  public void SetDefaultUsersFieldMappings()
  {
    foreach (var kvp in Settings.DefaultUsersFieldMappings)
    {
      _logger.Debug(
        "Attempting to find field {Name} in User app in Onspring",
        kvp.Value
      );

      var onspringField = _settings
      .Onspring
      .UsersFields
      .FirstOrDefault(
        f => f.Name == kvp.Value
      );

      var fieldId = onspringField?.Id ?? 0;

      if (
        onspringField is not null &&
        onspringField.Name is OnspringSettings.UsersUsernameField
      )
      {
        _settings.Onspring.UsersUsernameFieldId = fieldId;
      }

      if (
        onspringField is not null &&
        onspringField.Name is OnspringSettings.UsersGroupsField
      )
      {
        _settings.Onspring.UsersGroupsFieldId = fieldId;
      }

      if (
        onspringField is not null &&
        onspringField.Name is OnspringSettings.UsersStatusField &&
        onspringField is ListField statusListField
      )
      {
        _settings.Onspring.UsersStatusFieldId = fieldId;
        SetStatusListValues(statusListField);
      }

      // if the Onspring field is already mapped
      // to an Azure User property, skip it
      if (_settings.UsersFieldMappings.ContainsKey(fieldId))
      {
        _logger.Debug(
          "Onspring Field {Name} is already mapped to an Azure User property: {@FieldMappings}",
          kvp.Value,
          _settings.UsersFieldMappings
        );

        continue;
      }

      _logger.Debug(
        "Found field {Name} in User app in Onspring with Id {Id}",
        kvp.Value,
        fieldId
      );

      _logger.Debug(
        "Setting user field mapping: {Key} - {Value}",
        kvp.Key,
        fieldId
      );

      _settings
      .UsersFieldMappings
      .Add(
        fieldId,
        kvp.Key
      );
    }
  }

  public void SetDefaultGroupsFieldMappings()
  {
    _logger.Debug("Setting default Groups field mappings");

    foreach (var kvp in Settings.DefaultGroupsFieldMappings)
    {
      _logger.Debug(
        "Attempting to find field {Name} in Group app in Onspring",
        kvp.Value
      );

      var onspringField = _settings
      .Onspring
      .GroupsFields
      .FirstOrDefault(
        f => f.Name == kvp.Value
      );

      var fieldId = onspringField?.Id ?? 0;

      if (
        onspringField is not null &&
        onspringField.Name is OnspringSettings.GroupsNameField
      )
      {
        _settings.Onspring.GroupsNameFieldId = fieldId;
      }

      // if the property being mapped is the Azure Group id
      // property and the Onspring Group Name field is already
      // mapped to a Azure Group property, override
      // the existing mapping. Azure Group id is always mapped
      // to the Onspring Group Name field
      if (
        _settings.GroupsFieldMappings.ContainsKey(fieldId) &&
        kvp.Key == AzureSettings.GroupsNameKey
      )
      {
        _logger.Debug(
          "Overriding existing Azure Group id field mapping: {Key} - {Value}",
          kvp.Key,
          fieldId
        );

        _settings.GroupsFieldMappings[fieldId] = AzureSettings.GroupsNameKey;

        continue;
      }

      // if the Onspring Group field is already mapped to a
      // Azure Group property, skip it
      if (_settings.GroupsFieldMappings.ContainsKey(fieldId))
      {
        _logger.Debug(
          "Onspring Group field {Name} is already mapped to an Azure Group property: {@FieldMappings}",
          kvp.Value,
          _settings.GroupsFieldMappings
        );

        continue;
      }

      _logger.Debug(
        "Setting group field mapping: {Key} - {Value}",
        kvp.Key,
        fieldId
      );

      _settings
      .GroupsFieldMappings
      .Add(
        fieldId,
        kvp.Key
      );
    }
  }

  public async Task GetOnspringUserFields()
  {
    _settings.Onspring.UsersFields = await _onspringService.GetUserFields();
  }

  public async Task GetOnspringGroupFields()
  {
    _settings.Onspring.GroupsFields = await _onspringService.GetGroupFields();
  }

  public async Task<bool> VerifyConnections()
  {
    var onspringConnected = await _onspringService.IsConnected();

    var graphConnected = await _graphService.IsConnected();

    if (onspringConnected is false)
    {
      _logger.Error("Unable to connect to Onspring API");
    }

    if (graphConnected is false)
    {
      _logger.Error("Unable to connect to Azure AD Graph API");
    }

    return onspringConnected && graphConnected;
  }

  internal bool HasValidFieldTypeToPropertyTypeMappings()
  {
    var invalidMappings = new Dictionary<int, string>();

    foreach (var kvp in _settings.GroupsFieldMappings)
    {
      var onspringField = _settings
      .Onspring
      .GroupsFields
      .FirstOrDefault(
        f => f.Id == kvp.Key
      );

      var azureGroupProperty = _settings
      .Azure
      .GroupsProperties
      .FirstOrDefault(
        p => p.Name == kvp.Value.Capitalize()
      );

      if (
        azureGroupProperty is null ||
        onspringField is null
      )
      {
        _logger.Error(
          "Unable to find Onspring Group field {Id} or Azure Group property {Name}",
          kvp.Key,
          kvp.Value
        );

        continue;
      }

      if (IsValidFieldTypeAndPropertyType(onspringField, azureGroupProperty) is false)
      {
        invalidMappings.Add(kvp.Key, kvp.Value);
      }
    }

    foreach (var kvp in _settings.UsersFieldMappings)
    {
      if (
        kvp.Key == _settings.Onspring.UsersGroupsFieldId ||
        kvp.Key == 0
      )
      {
        continue;
      }

      var onspringField = _settings
      .Onspring
      .UsersFields
      .FirstOrDefault(
        f => f.Id == kvp.Key
      );

      var azureUserProperty = _settings
      .Azure
      .UsersProperties
      .FirstOrDefault(
        p => p.Name == kvp.Value.Capitalize()
      );

      if (
        azureUserProperty is null ||
        onspringField is null
      )
      {
        _logger.Error(
          "Unable to find Onspring User field {Id} or Azure User property {Name}",
          kvp.Key,
          kvp.Value
        );

        continue;
      }

      if (IsValidFieldTypeAndPropertyType(onspringField, azureUserProperty) is false)
      {
        invalidMappings.Add(kvp.Key, kvp.Value);
      }
    }

    if (invalidMappings.Any())
    {
      _logger.Error(
        "Invalid field type to property type mappings: {@InvalidMappings}",
        invalidMappings
      );

      return false;
    }

    return true;
  }

  internal static bool IsValidFieldTypeAndPropertyType(
    Field field,
    PropertyInfo azureProperty
  )
  {
    var type = azureProperty.PropertyType;

    // string values can be mapped to text or list fields
    // bool values can be mapped to text or list fields
    // DateTime values can be mapped to date or text fields
    // DateTimeOffset values can be mapped to date or text fields
    // List<string> values can be mapped to list fields that are multi-select
    // or text fields
    switch (type)
    {
      case var s when s == typeof(string):
      case var b when b == typeof(bool?):
        return field.Type is FieldType.Text or FieldType.List;

      case var dt when dt == typeof(DateTime?):
      case var dto when dto == typeof(DateTimeOffset?):
        return field.Type is FieldType.Date or FieldType.Text;

      case var t when t == typeof(List<string>):
        if (field.Type is FieldType.List)
        {
          var listField = (ListField) field;

          return listField.Type is FieldType.List &&
          listField.Multiplicity is Multiplicity.MultiSelect;
        }

        return field.Type is FieldType.Text;

      default:
        return false;
    }
  }

  internal bool HasRequiredOnspringFields()
  {
    var requiredGroupFieldIds = _settings
    .Onspring
    .GroupsFields
    .Where(
      f => _settings.Onspring.GroupRequiredFields.Contains(f.Name)
    )
    .Select(f => f.Id)
    .ToList();

    var requiredUserFieldIds = _settings
    .Onspring
    .UsersFields
    .Where(
      f => _settings.Onspring.UserRequiredFields.Contains(f.Name)
    )
    .Select(f => f.Id)
    .ToList();

    var hasRequiredOnspringGroupFields = requiredGroupFieldIds
    .All(
      _settings.GroupsFieldMappings.ContainsKey
    );

    var hasRequiredOnspringUserFields = requiredUserFieldIds
    .All(
      _settings.UsersFieldMappings.ContainsKey
    );

    if (hasRequiredOnspringGroupFields is false)
    {
      _logger.Debug(
        "Required Onspring Group Field not found in Groups Field Mappings: {@GroupsFieldMappings}",
        _settings.GroupsFieldMappings
      );
    }

    if (hasRequiredOnspringUserFields is false)
    {
      _logger.Debug(
        "Required Onspring User Field not found in Users Field Mappings: {@UsersFieldMappings}",
        _settings.UsersFieldMappings
      );
    }

    return hasRequiredOnspringGroupFields &&
    hasRequiredOnspringUserFields;
  }

  internal bool HasValidOnspringFields()
  {
    var onspringGroupFieldIds = _settings
    .Onspring
    .GroupsFields
    .Select(f => f.Id)
    .ToList();

    var onspringUserFieldIds = _settings
    .Onspring
    .UsersFields
    .Select(f => f.Id)
    .ToList();

    // 0 is a valid value for Onspring field ids in
    // this context because it means a required default
    // Azure AD property is being used but was not
    // mapped to a Onspring field
    var hasValidOnspringGroupFields = _settings
    .GroupsFieldMappings
    .All(
      kvp =>
        onspringGroupFieldIds
        .Contains(kvp.Key) ||
        kvp.Key == 0
    );

    var hasValidOnspringUserFields = _settings
    .UsersFieldMappings
    .All(
      kvp =>
        onspringUserFieldIds
        .Contains(kvp.Key) ||
        kvp.Key == 0
    );

    if (hasValidOnspringGroupFields is false)
    {
      _logger.Debug(
        "Invalid Onspring Group field id found in GroupsFieldMappings"
      );
    }

    if (hasValidOnspringUserFields is false)
    {
      _logger.Debug(
        "Invalid Onspring User field id found in UsersFieldMappings"
      );
    }

    return hasValidOnspringGroupFields && hasValidOnspringUserFields;
  }

  internal bool HasValidAzureProperties()
  {
    var azureGroupPropertyNames = _settings
    .Azure
    .GroupsProperties
    .Select(p => p.Name)
    .ToList();

    var azureUserPropertyNames = _settings
    .Azure
    .UsersProperties
    .Select(p => p.Name)
    .ToList();

    var hasValidAzureGroupProperties = _settings
    .GroupsFieldMappings
    .All(
      kvp =>
        azureGroupPropertyNames
        .Contains(kvp.Value.Capitalize())
    );

    var hasValidAzureUserProperties = _settings
    .UsersFieldMappings
    .All(
      kvp =>
        azureUserPropertyNames
        .Contains(kvp.Value.Capitalize())
    );

    if (hasValidAzureGroupProperties is false)
    {
      _logger.Debug(
        "Invalid Azure Group property name found in GroupsFieldMappings: {@GroupsFieldMappings}",
        _settings.GroupsFieldMappings
      );
    }

    if (hasValidAzureUserProperties is false)
    {
      _logger.Debug(
        "Invalid Azure User property name found in UsersFieldMappings: {@UsersFieldMappings}",
        _settings.UsersFieldMappings
      );
    }

    return hasValidAzureGroupProperties && hasValidAzureUserProperties;
  }

  internal async Task SyncUser(User azureUser)
  {
    _logger.Debug(
      "Syncing Azure User: {@AzureUser}",
      azureUser
    );

    var azureUserGroups = await _graphService.GetUserGroups(azureUser);

    if (azureUserGroups.Any() == false)
    {
      _logger.Warning(
        "No groups found for Azure User: {@AzureUser}",
        azureUser
      );
    }

    var usersGroupMappings = await GetUsersGroupMappings(azureUserGroups);

    var onspringUser = await _onspringService.GetUser(azureUser);

    if (onspringUser is null)
    {
      _logger.Debug(
        "Azure User not found in Onspring: {@AzureUser}",
        azureUser
      );

      _logger.Debug(
        "Attempting to create Azure User in Onspring: {@AzureUser}",
        azureUser
      );

      var createResponse = await _onspringService.CreateUser(
        azureUser,
        usersGroupMappings
      );

      if (createResponse is null)
      {
        _logger.Warning(
          "Unable to create Azure User in Onspring: {@AzureUser}",
          azureUser
        );

        return;
      }

      _logger.Debug(
        "Azure User {@AzureUser} created in Onspring: {@Response}",
        azureUser,
        createResponse
      );

      return;
    }

    _logger.Debug(
      "Azure User found in Onspring: {@OnspringUser}",
      onspringUser
    );

    _logger.Debug(
      "Attempting to update Azure User in Onspring: {@AzureUser}",
      azureUser
    );

    var updateResponse = await _onspringService.UpdateUser(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    if (updateResponse is null)
    {
      _logger.Warning(
        "Onspring User {@OnspringUser} not updated",
        onspringUser
      );

      return;
    }

    _logger.Debug(
      "Onspring User {@OnspringUser} updated: {@Response}",
      onspringUser,
      updateResponse
    );

    _logger.Debug(
      "Finished processing Azure User: {@AzureUser}",
      azureUser
    );
  }

  internal async Task SyncGroup(Group azureGroup)
  {
    _logger.Debug("Processing Azure AD Group: {@AzureGroup}", azureGroup);

    var onspringGroup = await _onspringService.GetGroup(azureGroup.Id);

    if (onspringGroup is null)
    {
      _logger.Debug(
        "Azure Group not found in Onspring: {@AzureGroup}",
        azureGroup
      );

      _logger.Debug(
        "Attempting to create Azure Group in Onspring: {@AzureGroup}",
        azureGroup
      );

      var createResponse = await _onspringService.CreateGroup(azureGroup);

      if (createResponse is null)
      {
        _logger.Warning(
          "Unable to create Azure Group in Onspring: {@AzureGroup}",
          azureGroup
        );

        return;
      }

      _logger.Debug(
        "Azure Group {@AzureGroup} created in Onspring: {@Response}",
        azureGroup,
        createResponse
      );

      return;
    }

    _logger.Debug(
      "Azure Group found in Onspring: {@OnspringGroup}",
      onspringGroup
    );

    _logger.Debug(
      "Updating Onspring Group: {@OnspringGroup}",
      onspringGroup
    );

    var updateResponse = await _onspringService.UpdateGroup(azureGroup, onspringGroup);

    if (updateResponse is null)
    {
      _logger.Warning(
        "Onspring Group {@OnspringGroup} not updated",
        onspringGroup
      );

      return;
    }

    _logger.Debug(
      "Onspring Group {@OnspringGroup} updated: {@Response}",
      onspringGroup,
      updateResponse
    );

    _logger.Debug(
      "Finished processing Azure AD Group: {@AzureGroup}",
      azureGroup
    );
  }

  internal async Task<Dictionary<string, int>> GetUsersGroupMappings(
    List<DirectoryObject> azureUserGroups
  )
  {
    var onspringGroupFields = await _onspringService.GetGroupFields();

    var recordIdField = onspringGroupFields
    .FirstOrDefault(
      f => f.Type is FieldType.AutoNumber
    );

    var groupMappings = new Dictionary<string, int>();

    foreach (var azureUserGroup in azureUserGroups)
    {
      if (
        azureUserGroup is null ||
        azureUserGroup.Id is null
      )
      {
        continue;
      }

      var onspringGroup = await _onspringService.GetGroup(azureUserGroup.Id);

      if (onspringGroup is null)
      {
        _logger.Warning(
          "Onspring Group not found for Azure User Group: {@AzureUserGroup}",
          azureUserGroup
        );

        continue;
      }

      groupMappings.Add(azureUserGroup.Id, onspringGroup.RecordId);
    }

    return groupMappings;
  }

  internal void SetStatusListValues(ListField statusListField)
  {
    var activeListValue = statusListField
        .Values
        .FirstOrDefault(
          v =>
            v.Name == OnspringSettings.UsersActiveStatusListValueName
        );

    var inactiveListValue = statusListField
    .Values
    .FirstOrDefault(
      v =>
        v.Name == OnspringSettings.UsersInactiveStatusListValueName
    );

    if (activeListValue is null)
    {
      _logger.Fatal(
        "Unable to find list value {Name} in field {Field} in Onspring",
        OnspringSettings.UsersActiveStatusListValueName,
        statusListField.Name
      );

      throw new ApplicationException(
        $"Unable to find list value {OnspringSettings.UsersActiveStatusListValueName} in field {statusListField.Name} in Onspring"
      );
    }

    if (inactiveListValue is null)
    {
      _logger.Fatal(
        "Unable to find list value {Name} in field {Field} in Onspring",
        OnspringSettings.UsersInactiveStatusListValueName,
        statusListField.Name
      );

      throw new ApplicationException(
        $"Unable to find list value {OnspringSettings.UsersInactiveStatusListValueName} in field {statusListField.Name} in Onspring"
      );
    }

    _settings.Onspring.UserActiveStatusListValue = activeListValue.Id;
    _settings.Onspring.UserInactiveStatusListValue = inactiveListValue.Id;
  }
}