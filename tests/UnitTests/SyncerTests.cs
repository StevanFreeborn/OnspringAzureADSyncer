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
      .Setup(static p => p.VerifyConnections())
      .ReturnsAsync(false);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);

    var result = await syncer.Run();

    result.Should().NotBe(0);
  }

  [Fact]
  public async Task Run_WhenCalledAndGroupFilterIsInvalid_ItShouldReturnNonZeroValue()
  {
    _processorMock
      .Setup(static p => p.VerifyConnections())
      .ReturnsAsync(true);

    _processorMock
      .Setup(static p => p.HasValidGroupFilter())
      .ReturnsAsync((false, "Invalid group filter"));

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);

    var result = await syncer.Run();

    result.Should().NotBe(0);
  }

  [Fact]
  public async Task Run_WhenCalledAndConnectionCanBeMadeToAzureAndOnspringAndFieldMappingsAreValid_ItShouldReturnZero()
  {
    _processorMock
      .Setup(static p => p.HasValidGroupFilter())
      .ReturnsAsync((true, string.Empty));

    _processorMock
      .Setup(static p => p.VerifyConnections())
      .ReturnsAsync(true);

    _processorMock
      .Setup(static p => p.FieldMappingsAreValid())
      .Returns(true);

    _processorMock
      .Setup(static p => p.SyncGroups())
      .ReturnsAsync([]);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);

    var result = await syncer.Run();

    result.Should().Be(0);
  }

  [Fact]
  public async Task Run_WhenCalledAndConnectionCanBeMadeToAzureAndOnspringAndFieldMappingsAreValid_ItShouldSetDefaultFieldMappings()
  {
    _processorMock
      .Setup(static p => p.HasValidGroupFilter())
      .ReturnsAsync((true, string.Empty));

    _processorMock
      .Setup(static p => p.VerifyConnections())
      .ReturnsAsync(true);

    _processorMock
      .Setup(static p => p.SyncGroups())
      .ReturnsAsync([]);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);

    await syncer.Run();

    _processorMock.Verify(static p => p.SetDefaultGroupsFieldMappings(), Times.Once);
  }

  [Fact]
  public async Task Run_WhenCalledAndConnectionCanBeMadeToAzureAndOnspringAndFieldMappingsAreValid_ItShouldSyncGroups()
  {
    _processorMock
      .Setup(static p => p.HasValidGroupFilter())
      .ReturnsAsync((true, string.Empty));

    _processorMock
      .Setup(static p => p.VerifyConnections())
      .ReturnsAsync(true);

    _processorMock
      .Setup(static p => p.FieldMappingsAreValid())
      .Returns(true);

    _processorMock
      .Setup(static p => p.SyncGroups())
      .ReturnsAsync([new Group()]);

    _processorMock
      .Setup(static p => p.SyncGroupMembers(It.IsAny<Group>()))
      .Returns(Task.CompletedTask);

    var syncer = new Syncer(_loggerMock.Object, _processorMock.Object);

    await syncer.Run();

    _processorMock.Verify(static p => p.SyncGroups(), Times.Once);
    _processorMock.Verify(static p => p.SyncGroupMembers(It.IsAny<Group>()), Times.Once);
  }
}