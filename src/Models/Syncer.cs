namespace OnspringAzureADSyncer.Models;

public class Syncer : ISyncer
{
  private readonly ILogger _logger;
  private readonly IProcessor _processor;

  public Syncer(ILogger logger, IProcessor processor)
  {
    _logger = logger;
    _processor = processor;
  }

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

    // TODO: Validate field mappings
    // TODO: Validate that mapped fields exist in Onspring
    // TODO: Validate that mapped properties exist for Azure AD resource

    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.WriteLine("Connected successfully to Onspring and Azure AD");
    _logger.Information("Connected successfully to Onspring and Azure AD");

    var setDefaultGroupsFieldsMsg = "Setting default Groups field mappings";
    Console.WriteLine(setDefaultGroupsFieldsMsg);
    _logger.Information(setDefaultGroupsFieldsMsg);

    await _processor.SetDefaultGroupsFieldMappings();

    var setDefaultUsersFieldsMsg = "Setting default Users field mappings";
    Console.WriteLine(setDefaultUsersFieldsMsg);
    _logger.Information(setDefaultUsersFieldsMsg);

    await _processor.SetDefaultUsersFieldMappings();

    var syncGroupsMsg = "Syncing groups";
    Console.WriteLine(syncGroupsMsg);
    _logger.Information(syncGroupsMsg);

    await _processor.SyncGroups();

    // TODO: Sync users
    var syncUsersMsg = "Syncing users";
    Console.WriteLine(syncUsersMsg);
    _logger.Information(syncUsersMsg);

    await _processor.SyncUsers();

    // TODO: Sync group memberships

    // TODO: Reconcile users status

    var exitMsg = "Syncer finished";
    Console.WriteLine(exitMsg);
    _logger.Information(exitMsg);

    await Log.CloseAndFlushAsync();

    return 0;
  }
}