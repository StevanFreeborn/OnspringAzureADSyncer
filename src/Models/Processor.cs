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
        foreach (var azureUser in azureUsers)
        {
          progressBar.Tick($"Processing Azure User: {azureUser.Id}");
          await SyncUser(azureUser);
        }

        progressBar.Message = $"Finished processing Azure Users: page {pageNumberProcessing}";
      }

      // clear the list before
      // the next iteration
      // to avoid processing a user
      // multiple times
      azureUsers.Clear();

    } while (userIterator.State != PagingState.Complete);
  }

  internal async Task SyncUser(User azureUser)
  {
    _logger.Debug(
      "Syncing Azure User: {@AzureUser}",
      azureUser
    );

    // TODO: get the azure user's group memberships
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

    var updateResponse = await _onspringService.UpdateUser(azureUser, onspringUser);

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
        foreach (var azureGroup in azureGroups)
        {
          progressBar.Tick($"Processing Azure Group: {azureGroup.Id}");
          await SyncGroup(azureGroup);
        }

        progressBar.Message = $"Finished processing Azure Groups: page {pageNumberProcessing}";
      }

      // clear the list before
      // the next iteration
      // to avoid processing a group
      // multiple times
      azureGroups.Clear();

    } while (groupIterator.State != PagingState.Complete);
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

  public async Task SetDefaultUsersFieldMappings()
  {
    _logger.Debug("Setting default field mappings");

    var onspringUserFields = await _onspringService.GetUserFields();

    foreach (var kvp in Settings.DefaultUsersFieldMappings)
    {
      _logger.Debug(
        "Attempting to find field {Name} in User app in Onspring",
        kvp.Value
      );

      var onspringField = onspringUserFields.FirstOrDefault(f => f.Name == kvp.Value);

      if (onspringField is null)
      {
        _logger.Error(
          "Unable to find field {Name} in Users app in Onspring",
          kvp.Value
        );

        _settings.UsersFieldMappings.Add(kvp.Key, 0);
        continue;
      }

      if (onspringField.Name is OnspringSettings.UsersUsernameField)
      {
        _settings.Onspring.UsersUsernameFieldId = onspringField.Id;
      }

      if (
        onspringField.Name is OnspringSettings.UsersStatusField &&
        onspringField is ListField statusListField
      )
      {
        _settings.Onspring.UsersStatusFieldId = onspringField.Id;
        SetStatusListValues(statusListField);
      }

      _logger.Debug(
        "Found field {Name} in User app in Onspring with Id {Id}",
        kvp.Value,
        onspringField.Id
      );

      _logger.Debug(
        "Setting user field mapping: {Key} - {Value}",
        kvp.Key,
        onspringField.Id
      );

      _settings.UsersFieldMappings.Add(kvp.Key, onspringField.Id);
    }
  }

  public async Task SetDefaultGroupsFieldMappings()
  {
    _logger.Debug("Setting default Groups field mappings");
    var onspringGroupFields = await _onspringService.GetGroupFields();

    foreach (var kvp in Settings.DefaultGroupsFieldMappings)
    {
      _logger.Debug(
        "Attempting to find field {Name} in Group app in Onspring",
        kvp.Value
      );

      var onspringField = onspringGroupFields.FirstOrDefault(f => f.Name == kvp.Value);

      if (onspringField is null)
      {
        _logger.Fatal(
          "Unable to find field {Name} in Group app in Onspring",
          kvp.Value
        );

        throw new ApplicationException(
          $"Unable to find field {kvp.Value} in Group app in Onspring"
        );
      }

      _logger.Debug(
        "Found field {Name} in Group app in Onspring with Id {Id}",
        kvp.Value,
        onspringField.Id
      );

      _logger.Debug(
        "Setting group field mapping: {Key} - {Value}",
        kvp.Key,
        onspringField.Id
      );

      _settings.GroupsFieldMappings.Add(kvp.Key, onspringField.Id);
    }
  }

  public async Task<bool> VerifyConnections()
  {
    _logger.Debug("Verifying connections to Onspring and Azure AD");

    _logger.Debug("Verifying connection to Onspring API");
    var onspringConnected = await _onspringService.IsConnected();

    _logger.Debug("Verifying connection to Azure AD Graph API");
    var graphConnected = await _graphService.IsConnected();

    if (onspringConnected is false)
    {
      _logger.Error("Unable to connect to Onspring API");
    }

    if (graphConnected is false)
    {
      _logger.Error("Unable to connect to Azure AD Graph API");
    }

    _logger.Debug("Connections verified");
    return onspringConnected && graphConnected;
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

    // TODO: for each group membership, get the onspring group's record id value
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