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