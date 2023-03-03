namespace OnspringAzureADSyncer.Models;

public class Processor : IProcessor
{
  private readonly ILogger _logger;
  private readonly Settings _settings;
  private readonly IOnspringService _onspringService;
  private readonly IGraphService _graphService;

  public Processor(
    ILogger logger,
    Settings settings,
    IOnspringService onspringService,
    IGraphService graphService
  )
  {
    _logger = logger;
    _settings = settings;
    _onspringService = onspringService;
    _graphService = graphService;
  }

  public async Task SyncGroups()
  {
    var azureGroups = new List<Group>();
    var pageSize = 1;
    var groupIterator = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    if (groupIterator is null)
    {
      _logger.Warning("No groups found in Azure AD");
      return;
    }

    var pageNumber = 0;

    do
    {
      if (groupIterator.State == PagingState.Paused)
      {
        await groupIterator.ResumeAsync();
        pageNumber++;
      }

      if (groupIterator.State == PagingState.NotStarted)
      {
        await groupIterator.IterateAsync();
        pageNumber++;
      }

      var options = new ProgressBarOptions
      {
        ForegroundColor = ConsoleColor.DarkBlue,
        ProgressCharacter = '─',
        ShowEstimatedDuration = false,
      };

      using (
        var progressBar = new ProgressBar(
          azureGroups.Count,
          $"Starting processing groups: page {pageNumber}",
          options
        )
      )
      {
        await Parallel.ForEachAsync(
          azureGroups,
          async (group, token) =>
          {
            await SyncGroup(group);
            progressBar.Tick($"Processing group: {group.DisplayName} - {group.Id}");
          }
        );

        progressBar.Message = $"Finished processing groups: page {pageNumber}";
      }

      // clear the list before
      // the next iteration
      // to avoid processing a group
      // multiple times
      azureGroups.Clear();

    } while (groupIterator.State != PagingState.Complete);
  }

  private async Task SyncGroup(Group group)
  {
    _logger.Debug("Processing Azure AD Group: {Name} - {Id}", group.DisplayName, group.Id);

    var onspringGroup = await _onspringService.GetGroup(group.Id);

    if (onspringGroup is null)
    {
      _logger.Debug("Group not found in Onspring: {Name} - {Id}", group.DisplayName, group.Id);
      _logger.Debug("Attempting to create group in Onspring: {Name} - {Id}", group.DisplayName, group.Id);

      var res = await _onspringService.CreateGroup(group);

      if (res is null)
      {
        _logger.Warning(
          "Unable to create group in Onspring: {Name} - {Id}",
          group.DisplayName,
          group.Id
        );

        return;
      }

      _logger.Debug(
        "Group {Name} - {Id} created in Onspring: {Response}",
        group.DisplayName,
        group.Id,
        res
      );

      return;
    }

    _logger.Debug("Group found in Onspring: {Name} - {Id}", group.DisplayName, group.Id);
    _logger.Debug("Updating Onspring Group: {Name} - {Id}", group.DisplayName, group.Id);
    // await _onspringService.UpdateGroup(group);
  }

  public async Task SetDefaultFieldMappings()
  {
    _logger.Debug("Setting default field mappings");
    var onspringGroupFields = await _onspringService.GetGroupFields();

    foreach (var kvp in Settings.DefaultGroupsFieldMappings)
    {
      _logger.Debug("Attempting to find field {Name} in Group app in Onspring", kvp.Value);
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
        "Found field {Name} in Group app in Onspring with ID {Id}",
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

    if (!onspringConnected)
    {
      _logger.Error("Unable to connect to Onspring API");
    }

    if (!graphConnected)
    {
      _logger.Error("Unable to connect to Azure AD Graph API");
    }

    _logger.Debug("Connections verified");
    return onspringConnected && graphConnected;
  }
}