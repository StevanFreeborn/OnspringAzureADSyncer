namespace OnspringAzureADSyncerTests.UnitTests;

public class SyncerTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<IProcessor> _processorMock;

  public SyncerTests()
  {
    _loggerMock = new Mock<ILogger>();
    _processorMock = new Mock<IProcessor>();
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