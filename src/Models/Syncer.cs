namespace OnspringAzureADSyncer.Models;

public class Syncer(ILogger logger, IProcessor processor) : ISyncer
{
  private readonly ILogger _logger = logger;
  private readonly IProcessor _processor = processor;

  public async Task<int> Run()
  {
    Console.ForegroundColor = ConsoleColor.DarkBlue;

    var startMsg = "Starting syncer";
    Console.WriteLine(startMsg);
    _logger.Information(startMsg);

    if (await _processor.VerifyConnections() is false)
    {
      Console.ForegroundColor = ConsoleColor.Red;

      var connErrorMsg = "Unable to connect to Onspring and/or Azure AD";
      Console.WriteLine(connErrorMsg);
      _logger.Fatal(connErrorMsg);

      return 2;
    }

    var (isGroupFilterValid, resultMessage) = await _processor.HasValidGroupFilter();

    if (isGroupFilterValid is false)
    {
      Console.ForegroundColor = ConsoleColor.Red;

      var invalidGroupFiltersMsg = "Group filter is invalid";
      Console.WriteLine(invalidGroupFiltersMsg);
      Console.WriteLine(resultMessage);
      _logger.Fatal(invalidGroupFiltersMsg);

      return 4;
    }

    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.WriteLine("Connected successfully to Onspring and Azure AD");
    _logger.Information("Connected successfully to Onspring and Azure AD");

    var retrieveGroupsFieldsMsg = "Retrieving fields for Onspring Groups app";
    Console.WriteLine(retrieveGroupsFieldsMsg);
    _logger.Information(retrieveGroupsFieldsMsg);

    await _processor.GetOnspringGroupFields();

    var retrieveUsersFieldsMsg = "Retrieving fields for Onspring Users app";
    Console.WriteLine(retrieveUsersFieldsMsg);
    _logger.Information(retrieveUsersFieldsMsg);

    await _processor.GetOnspringUserFields();

    var setDefaultGroupsFieldsMsg = "Setting default Groups field mappings";
    Console.WriteLine(setDefaultGroupsFieldsMsg);
    _logger.Information(setDefaultGroupsFieldsMsg);

    _processor.SetDefaultGroupsFieldMappings();

    var setDefaultUsersFieldsMsg = "Setting default Users field mappings";
    Console.WriteLine(setDefaultUsersFieldsMsg);
    _logger.Information(setDefaultUsersFieldsMsg);

    _processor.SetDefaultUsersFieldMappings();

    var validateFieldMappings = "Validating field mappings";
    Console.WriteLine(validateFieldMappings);
    _logger.Information(validateFieldMappings);

    if (_processor.FieldMappingsAreValid() is false)
    {
      Console.ForegroundColor = ConsoleColor.Red;

      var invalidCustomFieldMappingsMsg = "Field mappings are invalid";
      Console.WriteLine(invalidCustomFieldMappingsMsg);
      _logger.Fatal(invalidCustomFieldMappingsMsg);

      return 3;
    }

    var syncGroupsMsg = "Syncing groups";
    Console.WriteLine(syncGroupsMsg);
    _logger.Information(syncGroupsMsg);

    var syncedGroups = await _processor.SyncGroups();

    var syncGroupMembersMsg = "Syncing group members";
    Console.WriteLine(syncGroupMembersMsg);
    _logger.Information(syncGroupMembersMsg);

    foreach (var group in syncedGroups)
    {
      await _processor.SyncGroupMembers(group);
    }

    var exitMsg = "Syncer finished";
    Console.WriteLine(exitMsg);
    _logger.Information(exitMsg);

    await Log.CloseAndFlushAsync();

    return 0;
  }
}