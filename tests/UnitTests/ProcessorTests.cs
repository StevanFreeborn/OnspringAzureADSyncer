namespace OnspringAzureADSyncerTests.UnitTests;

public class ProcessorTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<ISettings> _settingsMock;
  private readonly Mock<IOnspringService> _onspringServiceMock;
  private readonly Mock<IGraphService> _graphServiceMock;

  private readonly Processor _processor;

  public ProcessorTests()
  {
    _loggerMock = new Mock<ILogger>();
    _settingsMock = new Mock<ISettings>();
    _onspringServiceMock = new Mock<IOnspringService>();
    _graphServiceMock = new Mock<IGraphService>();

    _processor = new Processor(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringServiceMock.Object,
      _graphServiceMock.Object
    );
  }

  [Fact]
  public async Task VerifyConnection_WhenCalledAndOnspringServiceAndGraphServiceAreConnected_ItShouldReturnTrue()
  {
    _onspringServiceMock
    .Setup(
      x => x.IsConnected()
    )
    .ReturnsAsync(true);

    _graphServiceMock
    .Setup(
      x => x.IsConnected().Result
    )
    .Returns(true);

    var result = await _processor.VerifyConnections();

    result.Should().BeTrue();
    _onspringServiceMock.Verify(
      x => x.IsConnected(),
      Times.Once
    );
    _graphServiceMock.Verify(
      x => x.IsConnected(),
      Times.Once
    );
  }

  [Fact]
  public async Task VerifyConnection_WhenCalledAndOnspringServiceIsNotConnected_ItShouldReturnFalse()
  {
    _onspringServiceMock
    .Setup(
      x => x.IsConnected().Result
    )
    .Returns(false);

    _graphServiceMock
    .Setup(
      x => x.IsConnected().Result
    )
    .Returns(true);

    var result = await _processor.VerifyConnections();

    result.Should().BeFalse();
    _onspringServiceMock.Verify(
      x => x.IsConnected(),
      Times.Once
    );
    _graphServiceMock.Verify(
      x => x.IsConnected(),
      Times.Once
    );
  }

  [Fact]
  public async Task VerifyConnection_WhenCalledAndGraphServiceIsNotConnected_ItShouldReturnFalse()
  {
    _onspringServiceMock
    .Setup(
      x => x.IsConnected().Result
    )
    .Returns(true);

    _graphServiceMock
    .Setup(
      x => x.IsConnected().Result
    )
    .Returns(false);

    var result = await _processor.VerifyConnections();

    result.Should().BeFalse();
    _onspringServiceMock.Verify(
      x => x.IsConnected(),
      Times.Once
    );
    _graphServiceMock.Verify(
      x => x.IsConnected(),
      Times.Once
    );
  }

  [Fact]
  public async Task SetDefaultFieldMappings_WhenCalled_ItShouldSetDefaultFieldMappings()
  {
    var groupFields = new List<Field>
    {
      new Field
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new Field
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
    };

    var configFile = new FileInfo("testData/testconfig.json");

    var optionsMock = new Mock<IOptions<AppOptions>>();
    optionsMock
    .SetupGet(
      x => x.Value
    )
    .Returns(
      new AppOptions
      {
        ConfigFile = configFile
      }
    );

    var settings = new Settings(optionsMock.Object)
    {
      GroupsFieldMappings = new Dictionary<string, int>()
    };

    var processor = new Processor(
      _loggerMock.Object,
      settings,
      _onspringServiceMock.Object,
      _graphServiceMock.Object
    );

    _onspringServiceMock
    .Setup(
      x => x.GetGroupFields().Result
    )
    .Returns(groupFields);

    await processor.SetDefaultFieldMappings();

    settings.GroupsFieldMappings.Should().NotBeEmpty();
    settings.GroupsFieldMappings.Should().HaveCount(2);
    settings.GroupsFieldMappings.Should().ContainKey("id");
    settings.GroupsFieldMappings.Should().ContainKey("description");
    settings.GroupsFieldMappings["id"].Should().Be(1);
    settings.GroupsFieldMappings["description"].Should().Be(2);
  }

  [Fact]
  public void SetDefaultFieldMappings_WhenCalledAndOnspringFieldCannotBeFound_ItShouldThrowException()
  {
    var groupFields = new List<Field>();

    _onspringServiceMock
    .Setup(
      x => x.GetGroupFields().Result
    )
    .Returns(groupFields);

    Task Act() => _processor.SetDefaultFieldMappings();

    Assert.ThrowsAsync<Exception>(Act);
  }

  [Fact]
  public async Task SyncGroups_WhenCalledAndNoGroupsAreFound_ItShouldNotSyncGroupsToOnspring()
  {
    var groups = new List<Group>();

    _graphServiceMock
    .Setup(
      x => x.GetGroupsIterator(It.IsAny<List<Group>>(), It.IsAny<int>()).Result
    )
    .Returns<PageIterator<Group, GroupCollectionResponse>>(null);

    await _processor.SyncGroups();

    _graphServiceMock.Verify(
      x => x.GetGroupsIterator(It.IsAny<List<Group>>(), It.IsAny<int>()),
      Times.Once
    );
    _onspringServiceMock.Verify(
      x => x.CreateGroup(It.IsAny<Group>()),
      Times.Never
    );
    _onspringServiceMock.Verify(
      x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()),
      Times.Never
    );
  }

  [Fact]
  public async Task SyncGroups_WhenCalledAndGroupsAreFound_ItShouldSyncGroupsToOnspring()
  {
    throw new NotImplementedException();
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsNotFound_ItShouldCreateGroupInOnspring()
  {
    throw new NotImplementedException();
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsNotFoundInOnspringButGroupIsNotCreatedInOnspring_ItShouldLogWarning()
  {
    throw new NotImplementedException();
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsFoundInOnspring_ItShouldUpdateGroupInOnspring()
  {
    throw new NotImplementedException();
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsFoundInOnspringAndGroupIsNotUpdated_ItShouldLogWarning()
  {
    throw new NotImplementedException();
  }
}