namespace OnspringAzureADSyncer.Models;

public class Processor : IProcessor
{
  private readonly ILogger _logger;
  private readonly IOnspringService _onspringService;
  private readonly IGraphService _graphService;

  public Processor(
    ILogger logger,
    IOnspringService onspringService,
    IGraphService graphService
  )
  {
    _logger = logger;
    _onspringService = onspringService;
    _graphService = graphService;
  }

  public async Task SyncGroups()
  {
    var azureGroups = new List<Group>();
    var pageSize = 50;
    var groupIterator = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    if (groupIterator is null)
    {
      _logger.Debug("No groups found in Azure AD");
      return;
    }

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

      await Parallel.ForEachAsync(
        azureGroups,
        async (group, token) => await SyncGroup(group)
      );

      // clear the list before
      // the next iteration
      // to avoid processing a group
      // multiple times
      azureGroups.Clear();

    } while (groupIterator.State != PagingState.Complete);
  }

  private async Task SyncGroup(Group group)
  {
    _logger.Information("Processing Azure AD Group: {Name} - {Id}", group.DisplayName, group.Id);

    var onspringGroup = await _onspringService.GetGroup(group.Id);

    if (onspringGroup is null)
    {
      _logger.Information("Group not found in Onspring: {Name} - {Id}", group.DisplayName, group.Id);
      _logger.Information("Creating Onspring Group: {Name} - {Id}", group.DisplayName, group.Id);
      // await _onspringService.CreateGroup(group);
      return;
    }

    _logger.Information("Group found in Onspring: {Name} - {Id}", group.DisplayName, group.Id);
    _logger.Information("Updating Onspring Group: {Name} - {Id}", group.DisplayName, group.Id);
    // await _onspringService.UpdateGroup(group);
  }

  public async Task<bool> VerifyConnections()
  {
    var onspringConnected = await _onspringService.IsConnected();
    var graphConnected = await _graphService.IsConnected();

    if (!onspringConnected)
    {
      _logger.Error("Unable to connect to Onspring API");
    }

    if (!graphConnected)
    {
      _logger.Error("Unable to connect to Azure AD Graph API");
    }

    return onspringConnected && graphConnected;
  }
}