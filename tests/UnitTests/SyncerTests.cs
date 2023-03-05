namespace OnspringAzureADSyncerTests.UnitTests;

public class SyncerTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<ISettings> _settingsMock;
  private readonly Mock<IAzureGroupDestructuringPolicy> _azureGroupDestructuringPolicyMock;
  private readonly Mock<IOnspringService> _onspringServiceMock;
  private readonly Mock<IGraphService> _graphServiceMock;
  private readonly Mock<IProcessor> _processorMock;
  private readonly Mock<ISyncer> _syncerMock;
  private readonly IHostBuilder _testHostBuilder;

  public SyncerTests()
  {
    _loggerMock = new Mock<ILogger>();
    _settingsMock = new Mock<ISettings>();
    _azureGroupDestructuringPolicyMock = new Mock<IAzureGroupDestructuringPolicy>();
    _onspringServiceMock = new Mock<IOnspringService>();
    _graphServiceMock = new Mock<IGraphService>();
    _processorMock = new Mock<IProcessor>();
    _syncerMock = new Mock<ISyncer>();

    _testHostBuilder = Host
    .CreateDefaultBuilder()
    .ConfigureServices(
      (context, services) =>
      {
        services.AddSingleton(_settingsMock.Object);
        services.AddSingleton(_azureGroupDestructuringPolicyMock.Object);
        services.AddSingleton(_onspringServiceMock);
        services.AddSingleton(_graphServiceMock.Object);
        services.AddSingleton(_processorMock.Object);
        services.AddSingleton(_syncerMock.Object);
      }
    );
  }

  [Fact]
  public async Task Start_WhenCalledWithConfigFilePathThatExists_ItShouldReturnZeroValue()
  {
    var configFileThatExists = Path.Combine(
      AppContext.BaseDirectory,
      "testData",
      "config.exists.json"
    );

    var options = new Options
    {
      ConfigFile = configFileThatExists,
      LogLevel = LogEventLevel.Debug,
    };

    var result = await Syncer.Start(_testHostBuilder, options);
    result.Should().Be(0);
  }

  [Fact]
  public async Task Start_WhenCalledWithConfigFilePathThatDoesNotExist_ItShouldReturnNonZeroValue()
  {
    var configFileThatDoesNotExist = Path.Combine(
      AppContext.BaseDirectory,
      "testData",
      "config.does-not-exist.json"
    );

    var options = new Options
    {
      ConfigFile = configFileThatDoesNotExist,
      LogLevel = LogEventLevel.Debug,
    };

    var result = await Syncer.Start(_testHostBuilder, options);
    result.Should().NotBe(0);
  }

  [Fact]
  public async Task Run_WhenCalledAndConnectionCanNotBeMadeToEitherAzureOrOnspring_ItShouldReturnNonZeroValue()
  {
    _processorMock
    .Setup(p => p.VerifyConnections().Result)
    .Returns(false);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);
    var result = await syncer.Run();
    result.Should().NotBe(0);
  }

  [Fact]
  public async Task Run_WhenCalledAndConnectionCanBeMadeToAzureAndOnspring_ItShouldReturnZero()
  {
    _processorMock
    .Setup(p => p.VerifyConnections().Result)
    .Returns(true);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);
    var result = await syncer.Run();
    result.Should().Be(0);
  }

  [Fact]
  public async Task Run_WhenCalledAndConnectionCanBeMadeToAzureAndOnspring_ItShouldSetDefaultFieldMappings()
  {
    _processorMock
    .Setup(p => p.VerifyConnections().Result)
    .Returns(true);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);
    await syncer.Run();
    _processorMock.Verify(p => p.SetDefaultFieldMappings(), Times.Once);
  }

  [Fact]
  public async Task Run_WhenCalledAndConnectionCanBeMadeToAzureAndOnspring_ItShouldSyncGroups()
  {
    _processorMock
    .Setup(p => p.VerifyConnections().Result)
    .Returns(true);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);
    await syncer.Run();
    _processorMock.Verify(p => p.SyncGroups(), Times.Once);
  }
}