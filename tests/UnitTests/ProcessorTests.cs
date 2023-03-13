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
  public void SetDefaultGroupsFieldMappings_WhenCalled_ItShouldSetDefaultGroupsFieldMappings()
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
      Onspring = new OnspringSettings
      {
        GroupsFields = groupFields
      }
    };

    var processor = new Processor(
      _loggerMock.Object,
      settings,
      _onspringServiceMock.Object,
      _graphServiceMock.Object
    );

    processor.SetDefaultGroupsFieldMappings();

    settings.GroupsFieldMappings.Should().NotBeEmpty();
    settings.GroupsFieldMappings.Should().HaveCount(2);
    settings.GroupsFieldMappings.Should().ContainKey(1);
    settings.GroupsFieldMappings.Should().ContainKey(2);
    settings.GroupsFieldMappings[1].Should().Be("id");
    settings.GroupsFieldMappings[2].Should().Be("description");
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
      x => x.GetGroup(It.IsAny<string>()),
      Times.Never
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

  // Note: This test is complicated by the fact
  // we are using the page iterator class to iterator over groups
  // and then also using an wrapper class on the actual
  // graphServiceClient to make unit testing possible
  [Fact]
  public async Task SyncGroups_WhenCalledAndGroupsAreFound_ItShouldSyncGroupsToOnspring()
  {
    // setup azure groups to sync
    var azureGroups = new List<Group>
    {
      new Group // will create this group
      {
        Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
        Description = "Test Group 1",
      },
      new Group // will use this group to update onspring group
      {
        Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
        Description = "Test Group 2",
      },
    };

    // setup azure groups collection to return
    // as initial groups when creating page iterator
    var azureGroupCollection = new GroupCollectionResponse
    {
      Value = azureGroups
    };

    // setup onspring group that we will
    // pretend was found by onspring service
    var onspringGroup = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new()
      {
        new StringFieldValue(1, "1f01a3d4-7142-4210-b54d-9aadf98ce929"),
        new StringFieldValue(2, "Needs updated")
      }
    };

    // setup msGraph that we can mock
    // to return initial set of groups
    // to create page iterator
    var msGraphMock = new Mock<IMsGraph>();

    // mock to return collection of groups
    msGraphMock
    .Setup(
      x => x.GetGroupsForIterator(
        It.IsAny<Dictionary<int, string>>()
      ).Result
    )
    .Returns(azureGroupCollection);

    // mock graph service client for msGraphMock
    var tokenCredentialMock = new Mock<TokenCredential>();
    msGraphMock
    .SetupGet(
      x => x.GraphServiceClient
    )
    .Returns(
      new GraphServiceClient(
        tokenCredentialMock.Object,
        null,
        null
      )
    );

    // create new graph service using 
    // the test specific msGraphMock
    var graphService = new GraphService(
      _loggerMock.Object,
      _settingsMock.Object,
      msGraphMock.Object
    );

    _settingsMock
    .SetupGet(
      x => x.Onspring
    ).Returns(
      new OnspringSettings
      {
        GroupsFields = new List<Field>()
      }
    );

    _settingsMock
    .SetupGet(
      x => x.GroupsFieldMappings
    )
    .Returns(
      new Dictionary<int, string>()
    );

    // setup onspring service
    // to mock returning nul for first
    // azure group and a found onspring
    // group for the second azure group
    _onspringServiceMock
    .SetupSequence(
      x => x.GetGroup(It.IsAny<string>()).Result
    )
    .Returns((ResultRecord?) null)
    .Returns(onspringGroup);

    // create new instance of processor
    // for this specific test to use
    // the test specific graph service
    var processor = new Processor(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringServiceMock.Object,
      graphService
    );

    await processor.SyncGroups();

    _onspringServiceMock.Verify(
      x => x.GetGroup(It.IsAny<string>()),
      Times.Exactly(2)
    );
    _onspringServiceMock.Verify(
      x => x.CreateGroup(It.IsAny<Group>()),
      Times.Once
    );
    _onspringServiceMock.Verify(
      x => x.UpdateGroup(
        It.IsAny<Group>(),
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsNotFound_ItShouldCreateGroupInOnspring()
  {
    var azureGroup = new Group
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      Description = "Test Group 1",
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = new List<string>()
    };

    _onspringServiceMock
    .Setup(
      x => x.GetGroup(It.IsAny<string>()).Result
    )
    .Returns<ResultRecord?>(null);

    _onspringServiceMock
    .Setup(
      x => x.CreateGroup(It.IsAny<Group>()).Result
    )
    .Returns(
      saveRecordResponse
    );

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(
      x => x.CreateGroup(It.IsAny<Group>()),
      Times.Once
    );

    _onspringServiceMock.Verify(
      x => x.UpdateGroup(
        It.IsAny<Group>(),
        It.IsAny<ResultRecord>()
      ),
      Times.Never
    );
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsNotFoundInOnspringButGroupIsNotCreatedInOnspring_ItShouldLogWarning()
  {
    var azureGroup = new Group
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      Description = "Test Group 1",
    };

    _onspringServiceMock
    .Setup(
      x => x.GetGroup(It.IsAny<string>()).Result
    )
    .Returns<ResultRecord?>(null);

    _onspringServiceMock
    .Setup(
      x => x.CreateGroup(It.IsAny<Group>()).Result
    )
    .Returns<SaveRecordResponse?>(null);

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(
      x => x.CreateGroup(It.IsAny<Group>()),
      Times.Once
    );

    _loggerMock.Verify(
      x => x.Warning(
        It.IsAny<string>(),
        It.IsAny<Group>()
      ),
      Times.Once
    );

    _onspringServiceMock.Verify(
      x => x.UpdateGroup(
        It.IsAny<Group>(),
        It.IsAny<ResultRecord>()
      ),
      Times.Never
    );
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsFoundInOnspring_ItShouldUpdateGroupInOnspring()
  {
    var azureGroup = new Group
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      Description = "Test Group 1",
    };

    var resultRecord = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "98e58dab-9f2c-4216-bc91-70d7dabe227e"),
        new StringFieldValue(2, "Group that needs updating"),
      }
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = new List<string>()
    };

    _onspringServiceMock
    .Setup(
      x => x.GetGroup(It.IsAny<string>()).Result
    )
    .Returns(resultRecord);

    _onspringServiceMock
    .Setup(
      x => x.UpdateGroup(
        It.IsAny<Group>(),
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(
      saveRecordResponse
    );

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(
      x => x.CreateGroup(It.IsAny<Group>()),
      Times.Never
    );

    _onspringServiceMock.Verify(
      x => x.UpdateGroup(
        It.IsAny<Group>(),
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsFoundInOnspringAndGroupIsNotUpdated_ItShouldLogWarning()
  {
    var azureGroup = new Group
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      Description = "Test Group 1",
    };

    var resultRecord = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "98e58dab-9f2c-4216-bc91-70d7dabe227e"),
        new StringFieldValue(2, "Group that needs updating"),
      }
    };

    _onspringServiceMock
    .Setup(
      x => x.GetGroup(It.IsAny<string>()).Result
    )
    .Returns(resultRecord);

    _onspringServiceMock
    .Setup(
      x => x.UpdateGroup(
        It.IsAny<Group>(),
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns<ResultRecord?>(null);

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(
      x => x.CreateGroup(It.IsAny<Group>()),
      Times.Never
    );

    _onspringServiceMock.Verify(
      x => x.UpdateGroup(
        It.IsAny<Group>(),
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );

    _loggerMock.Verify(
      x => x.Warning(
        It.IsAny<string>(),
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }
}