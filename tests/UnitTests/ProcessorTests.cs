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
      .Setup(static x => x.IsConnected())
      .ReturnsAsync(true);

    _graphServiceMock
      .Setup(static x => x.IsConnected())
      .ReturnsAsync(true);

    var result = await _processor.VerifyConnections();

    result.Should().BeTrue();
    _onspringServiceMock.Verify(static x => x.IsConnected(), Times.Once);
    _graphServiceMock.Verify(static x => x.IsConnected(), Times.Once);
  }

  [Fact]
  public async Task VerifyConnection_WhenCalledAndOnspringServiceIsNotConnected_ItShouldReturnFalse()
  {
    _onspringServiceMock
      .Setup(static x => x.IsConnected())
      .ReturnsAsync(false);

    _graphServiceMock
      .Setup(static x => x.IsConnected())
      .ReturnsAsync(true);

    var result = await _processor.VerifyConnections();

    result.Should().BeFalse();
    _onspringServiceMock.Verify(static x => x.IsConnected(), Times.Once);
    _graphServiceMock.Verify(static x => x.IsConnected(), Times.Once);
  }

  [Fact]
  public async Task VerifyConnection_WhenCalledAndGraphServiceIsNotConnected_ItShouldReturnFalse()
  {
    _onspringServiceMock
      .Setup(static x => x.IsConnected())
      .ReturnsAsync(true);

    _graphServiceMock
      .Setup(static x => x.IsConnected())
      .ReturnsAsync(false);

    var result = await _processor.VerifyConnections();

    result.Should().BeFalse();
    _onspringServiceMock.Verify(static x => x.IsConnected(), Times.Once);
    _graphServiceMock.Verify(static x => x.IsConnected(), Times.Once);
  }

  [Fact]
  public void SetDefaultGroupsFieldMappings_WhenCalled_ItShouldSetDefaultGroupsFieldMappings()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
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
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = configFile
      });

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
  public void SetDefaultGroupsFieldMappings_WhenCalledAndDefaultFieldIsNotFound_ItShouldSetDefaultGroupsFieldMappingsWithFieldIdOfZero()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    var configFile = new FileInfo("testData/testconfig.json");

    var optionsMock = new Mock<IOptions<AppOptions>>();

    optionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = configFile
      });

    var settings = new Settings(optionsMock.Object)
    {
      Onspring = new OnspringSettings
      {
        GroupsFields = groupFields
      },
      Azure = new AzureSettings()
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
    settings.GroupsFieldMappings.Should().ContainKey(0);
    settings.GroupsFieldMappings[1].Should().Be("id");
    settings.GroupsFieldMappings[0].Should().Be("description");
  }

  [Fact]
  public void SetDefaultGroupsFieldMappings_WhenCalledAndNameFieldIsAlreadyMapped_ItShouldNotOverwriteTheExistingNameMapping()
  {

    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    var configFile = new FileInfo("testData/testconfig.json");

    var optionsMock = new Mock<IOptions<AppOptions>>();
    optionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = configFile
      });

    var settings = new Settings(optionsMock.Object)
    {
      Onspring = new OnspringSettings
      {
        GroupsFields = groupFields
      },
      Azure = new AzureSettings(),
      GroupsFieldMappings = new Dictionary<int, string>
      {
        { 1, "someProperty" }
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
    settings.GroupsFieldMappings[1].Should().Be("someProperty");
    settings.GroupsFieldMappings[2].Should().Be("description");
  }

  [Fact]
  public void SetDefaultGroupsFieldMappings_WhenCalledAndDescriptionFieldIsAlreadyMapped_ItShouldIgnoreTheDefaultMapping()
  {

    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    var configFile = new FileInfo("testData/testconfig.json");

    var optionsMock = new Mock<IOptions<AppOptions>>();

    optionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = configFile
      });

    var settings = new Settings(optionsMock.Object)
    {
      Onspring = new OnspringSettings
      {
        GroupsFields = groupFields
      },
      Azure = new AzureSettings(),
      GroupsFieldMappings = new Dictionary<int, string>
      {
        { 2, "someProperty" }
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
    settings.GroupsFieldMappings[2].Should().Be("someProperty");
  }

  [Fact]
  public void SetDefaultUsersFieldMappings_WhenCalled_ItShouldSetDefaultUsersFieldMappings()
  {
    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    var userFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new ListField
      {
        Id = 5,
        AppId = 1,
        Name = "Status",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        ListId = 1,
        Multiplicity = Multiplicity.SingleSelect,
        Values =
        [
          new ListValue
          {
            Id = activeListValueId,
            Name = "Active",
          },
          new ListValue
          {
            Id = inactiveListValueId,
            Name = "Inactive",
          }
        ]
      },
      new()
      {
        Id = 6,
        AppId = 1,
        Name = "Groups",
        Type = FieldType.Reference,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    var configFile = new FileInfo("testData/testconfig.json");

    var optionsMock = new Mock<IOptions<AppOptions>>();

    optionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = configFile
      });

    var settings = new Settings(optionsMock.Object)
    {
      Onspring = new OnspringSettings
      {
        UsersFields = userFields,
      },
      Azure = new AzureSettings()
    };

    var processor = new Processor(
      _loggerMock.Object,
      settings,
      _onspringServiceMock.Object,
      _graphServiceMock.Object
    );

    processor.SetDefaultUsersFieldMappings();

    settings.UsersFieldMappings.Should().NotBeEmpty();
    settings.UsersFieldMappings.Should().HaveCount(7);
    settings.UsersFieldMappings.Should().ContainKey(0);
    settings.UsersFieldMappings.Should().ContainKey(1);
    settings.UsersFieldMappings.Should().ContainKey(2);
    settings.UsersFieldMappings.Should().ContainKey(3);
    settings.UsersFieldMappings.Should().ContainKey(4);
    settings.UsersFieldMappings.Should().ContainKey(5);
    settings.UsersFieldMappings.Should().ContainKey(6);
    settings.UsersFieldMappings[0].Should().Be("id");
    settings.UsersFieldMappings[1].Should().Be("userPrincipalName");
    settings.UsersFieldMappings[2].Should().Be("givenName");
    settings.UsersFieldMappings[3].Should().Be("surname");
    settings.UsersFieldMappings[4].Should().Be("mail");
    settings.UsersFieldMappings[5].Should().Be("accountEnabled");
    settings.UsersFieldMappings[6].Should().Be("memberOf");
    settings.Onspring.UsersUsernameFieldId = 1;
    settings.Onspring.UsersStatusFieldId = 5;
    settings.Onspring.UsersGroupsFieldId = 6;
    settings.Onspring.UserActiveStatusListValue = activeListValueId;
    settings.Onspring.UserInactiveStatusListValue = inactiveListValueId;
  }

  [Fact]
  public void SetDefaultUsersFieldMappings_WhenCalledAndDefaultFieldsAreNotFound_ItShouldSetOnlyOneDefaultFieldWithFieldIdOfZero()
  {
    var userFields = new List<Field>();

    var configFile = new FileInfo("testData/testconfig.json");

    var optionsMock = new Mock<IOptions<AppOptions>>();

    optionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = configFile
      });


    var settings = new Settings(optionsMock.Object)
    {
      Onspring = new OnspringSettings
      {
        UsersFields = userFields,
      },
      Azure = new AzureSettings()
    };

    var processor = new Processor(
      _loggerMock.Object,
      settings,
      _onspringServiceMock.Object,
      _graphServiceMock.Object
    );

    processor.SetDefaultUsersFieldMappings();

    settings.UsersFieldMappings.Should().NotBeEmpty();
    settings.UsersFieldMappings.Should().HaveCount(1);
    settings.UsersFieldMappings.Should().ContainKey(0);
    settings.UsersFieldMappings[0].Should().Be("id");
  }

  [Fact]
  public void SetDefaultUsersFieldMappings_WhenCalledAndDefaultFieldsAreAlreadyMapped_ItShouldIgnoreDefaultMappings()
  {
    var userFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 5,
        AppId = 1,
        Name = "Status",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 6,
        AppId = 1,
        Name = "Groups",
        Type = FieldType.Reference,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    var configFile = new FileInfo("testData/testconfig.json");

    var optionsMock = new Mock<IOptions<AppOptions>>();

    optionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = configFile
      });


    var settings = new Settings(optionsMock.Object)
    {
      Onspring = new OnspringSettings
      {
        UsersFields = userFields,
      },
      Azure = new AzureSettings(),
      UsersFieldMappings = new Dictionary<int, string>
      {
        { 10, "id" },
        { 1, "someProperty" },
        { 2, "someProperty" },
        { 3, "someProperty" },
        { 4, "someProperty" },
        { 5, "someProperty" },
        { 6, "someProperty" },
      }
    };

    var processor = new Processor(
      _loggerMock.Object,
      settings,
      _onspringServiceMock.Object,
      _graphServiceMock.Object
    );

    processor.SetDefaultUsersFieldMappings();

    settings.UsersFieldMappings.Should().NotBeEmpty();
    settings.UsersFieldMappings.Should().HaveCount(8);
    settings.UsersFieldMappings.Should().ContainKey(10);
    settings.UsersFieldMappings.Should().ContainKey(1);
    settings.UsersFieldMappings.Should().ContainKey(2);
    settings.UsersFieldMappings.Should().ContainKey(3);
    settings.UsersFieldMappings.Should().ContainKey(4);
    settings.UsersFieldMappings.Should().ContainKey(5);
    settings.UsersFieldMappings.Should().ContainKey(6);
    settings.UsersFieldMappings[10].Should().Be("id");
    settings.UsersFieldMappings[1].Should().Be("someProperty");
    settings.UsersFieldMappings[2].Should().Be("someProperty");
    settings.UsersFieldMappings[3].Should().Be("someProperty");
    settings.UsersFieldMappings[4].Should().Be("someProperty");
    settings.UsersFieldMappings[5].Should().Be("someProperty");
    settings.UsersFieldMappings[6].Should().Be("someProperty");
  }

  [Fact]
  public async Task GetOnspringUserFields_WhenCalled_ItShouldSetTheOnspringUserFields()
  {
    var userFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    var onspringSettings = new OnspringSettings
    {
      UsersFields = []
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(onspringSettings);

    _onspringServiceMock
      .Setup(static x => x.GetUserFields())
      .ReturnsAsync(userFields);

    await _processor.GetOnspringUserFields();

    _onspringServiceMock.Verify(static x => x.GetUserFields(), Times.Once);

    onspringSettings.UsersFields.Should().NotBeEmpty();
    onspringSettings.UsersFields.Should().HaveCount(2);
    onspringSettings.UsersFields.Should().BeEquivalentTo(userFields);
  }

  [Fact]
  public async Task GetOnspringGroupsFields_WhenCalled_ItShouldSetTheOnspringGroupsFields()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    var onspringSettings = new OnspringSettings
    {
      GroupsFields = []
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(onspringSettings);

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync(groupFields);

    await _processor.GetOnspringGroupFields();

    _onspringServiceMock.Verify(static x => x.GetGroupFields(), Times.Once);

    onspringSettings.GroupsFields.Should().NotBeEmpty();
    onspringSettings.GroupsFields.Should().HaveCount(2);
    onspringSettings.GroupsFields.Should().BeEquivalentTo(groupFields);
  }

  [Fact]
  public async Task GetUsersGroupMappings_WhenCalled_ItShouldSetTheUsersGroupMappings()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Record Id",
        Type = FieldType.AutoNumber,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync(groupFields);

    var azureGroups = new List<Group>
    {
      new()
      {
        Id = "1",
        DisplayName = "Group 1",
        Description = "Group 1 Description",
      },
      new()
      {
        Id = "2",
        DisplayName = "Group 2",
        Description = "Group 2 Description",
      },
      null!,
      new()
      {
        Id = null,
        DisplayName = "Group 3",
        Description = "Group 3 Description",
      },
    };

    var onspringGroup = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = [],
    };

    _onspringServiceMock
      .SetupSequence(static x => x.GetGroup(It.IsAny<Group>()))
      .ReturnsAsync(onspringGroup)
      .ReturnsAsync((ResultRecord?) null);

    var usersGroupMappings = await _processor.GetUsersGroupMappings(azureGroups);

    usersGroupMappings.Should().NotBeEmpty();
    usersGroupMappings.Should().HaveCount(1);
    usersGroupMappings.Should().ContainKey("1");
    usersGroupMappings["1"].Should().Be(1);

    _onspringServiceMock.Verify(static x => x.GetGroupFields(), Times.Once);
    _onspringServiceMock.Verify(static x => x.GetGroup(It.IsAny<Group>()), Times.Exactly(2));
  }

  [Fact]
  public void SetStatusListValues_WhenCalledAndStatusListValuesAreFound_ItShouldSetTheUsersStatusListValues()
  {
    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    var statusListField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "Status",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Values = [
        new ListValue
        {
          Id = activeListValueId,
          Name = "Active",
        },
        new ListValue
        {
          Id = inactiveListValueId,
          Name = "Inactive",
        },
      ],
    };

    var onspringSettings = new OnspringSettings();

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(onspringSettings);

    _processor.SetStatusListValues(statusListField);

    onspringSettings.UserActiveStatusListValue.Should().Be(activeListValueId);
    onspringSettings.UserInactiveStatusListValue.Should().Be(inactiveListValueId);
  }

  [Fact]
  public void SetStatusListValues_WhenCalledAndActiveStatusListValueIsNotFound_ItShouldThrowAnException()
  {
    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    var statusListField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "Status",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Values = [
        new ListValue
        {
          Id = activeListValueId,
          Name = "Enabled",
        },
        new ListValue
        {
          Id = inactiveListValueId,
          Name = "Inactive",
        },
      ],
    };

    var act = () => _processor.SetStatusListValues(statusListField);

    act.Should().Throw<Exception>();
  }

  [Fact]
  public void SetStatusListValues_WhenCalledAndInactiveStatusListValueIsNotFound_ItShouldThrowAnException()
  {
    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    var statusListField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "Status",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Values =
      [
        new ListValue
        {
          Id = activeListValueId,
          Name = "Active",
        },
        new ListValue
        {
          Id = inactiveListValueId,
          Name = "Disabled",
        },
      ],
    };

    var act = () => _processor.SetStatusListValues(statusListField);

    act.Should().Throw<Exception>();
  }

  [Fact]
  public void FieldMappingsAreValid_WhenCalledAndFieldMappingsAreValid_ItShouldReturnTrue()
  {
    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = [
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
        ],
        UsersFields = [
          new Field
          {
            Id = 1,
            AppId = 1,
            Name = "Username",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 2,
            AppId = 1,
            Name = "First Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 3,
            AppId = 1,
            Name = "Last Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 4,
            AppId = 1,
            Name = "Email Address",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
        ],
      }
    );

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "displayName" },
        { 2, "description" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
        { 3, "surname" },
        { 4, "mail" },
      });

    var result = _processor.FieldMappingsAreValid();

    result.Should().BeTrue();
  }

  [Fact]
  public void FieldMappingsAreValid_WhenCalledAndFieldMappingsAreNotValid_ItShouldReturnFalse()
  {
    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = [
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
        ],
        UsersFields = [
          new Field
          {
            Id = 1,
            AppId = 1,
            Name = "Username",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 2,
            AppId = 1,
            Name = "First Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 3,
            AppId = 1,
            Name = "Last Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 4,
            AppId = 1,
            Name = "Email Address",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
        ],
      }
    );

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "displayName" },
        { 2, "invalid" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
        { 3, "surname" },
        { 4, "mail" },
      });

    var result = _processor.FieldMappingsAreValid();

    result.Should().BeFalse();
  }

  [Fact]
  public void HasValidAzureProperties_WhenCalledAndHasValidProperties_ItShouldReturnTrue()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "displayName" },
        { 2, "description" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "givenName" },
        { 2, "surname" },
        { 3, "mail" },
        { 4, "userPrincipalName" },
      });

    var result = _processor.HasValidAzureProperties();

    result.Should().BeTrue();
  }

  [Fact]
  public void HasValidAzureProperties_WhenCalledAndHasInvalidProperties_ItShouldReturnFalse()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "displayName" },
        { 2, "description" },
        { 3, "invalid" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "givenName" },
        { 2, "surname" },
        { 3, "mail" },
        { 4, "userPrincipalName" },
        { 5, "invalid" },
      });

    var result = _processor.HasValidAzureProperties();

    result.Should().BeFalse();
  }

  [Fact]
  public void HasValidAzureProperties_WhenCalledPropertyNamesAreDifferentCaseAndValid_ItShouldReturnTrue()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "DisplayName" },
        { 2, "DescRipTion" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "givenname" },
        { 2, "SURNAME" },
        { 3, "maiL" },
        { 4, "UserPrincipalName" },
      });

    var result = _processor.HasValidAzureProperties();

    result.Should().BeTrue();
  }

  [Fact]
  public void HasValidOnspringFields_WhenCalledAndHasValidFields_ItShouldReturnTrue()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = false,
        IsUnique = false,
      },
    };

    var userFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = groupFields,
        UsersFields = userFields,
      });

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "displayName" },
        { 2, "description" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "givenName" },
        { 2, "surname" },
        { 3, "mail" },
        { 4, "userPrincipalName" },
      });

    var result = _processor.HasValidOnspringFields();

    result.Should().BeTrue();
  }

  [Fact]
  public void HasValidOnspringFields_WhenCalledAndHasInvalidFields_ItShouldReturnFalse()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = false,
        IsUnique = false,
      },
    };

    var userFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = groupFields,
        UsersFields = userFields,
      });

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "displayName" },
        { 2, "description" },
        { 3, "invalid" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "givenName" },
        { 2, "surname" },
        { 3, "mail" },
        { 4, "userPrincipalName" },
        { 5, "invalid" },
      });

    var result = _processor.HasValidOnspringFields();

    result.Should().BeFalse();
  }

  [Fact]
  public void HasRequiredOnspringFields_WhenCalledAndAllRequiredFieldsAreMapped_ItShouldReturnTrue()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = false,
        IsUnique = false,
      },
    };

    var usersFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = groupFields,
        UsersFields = usersFields,
      });

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "displayName" },
        { 2, "description" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "givenName" },
        { 2, "surname" },
        { 3, "mail" },
        { 4, "userPrincipalName" },
      });

    var result = _processor.HasRequiredOnspringFields();

    result.Should().BeTrue();
  }

  [Fact]
  public void HasRequiredOnspringFields_WhenCalledAndRequiredFieldsAreNotMapped_ItShouldReturnFalse()
  {
    var groupFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = false,
        IsUnique = false,
      },
    };

    var usersFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = false,
      },
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = groupFields,
        UsersFields = usersFields,
      });

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 2, "description" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "givenName" },
        { 2, "surname" },
        { 3, "mail" },
      });

    var result = _processor.HasRequiredOnspringFields();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsStringAndFieldTypeIsText_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Text Field",
      Type = FieldType.Text,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(Group).GetProperty("DisplayName");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsStringAndFieldTypeIsList_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true
    };

    var azurePropertyInfo = typeof(Group).GetProperty("DisplayName");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsStringAndFieldTypeIsNotListOrText_ItShouldReturnFalse()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Date Field",
      Type = FieldType.Date,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(Group).GetProperty("DisplayName");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsBooleanAndFieldTypeIsText_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Text Field",
      Type = FieldType.Text,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(User).GetProperty("AccountEnabled");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsBooleanAndFieldTypeIsList_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(User).GetProperty("AccountEnabled");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsBooleanAndFieldTypeIsNotListOrText_ItShouldReturnFalse()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Date Field",
      Type = FieldType.Date,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(User).GetProperty("AccountEnabled");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsDateTimeAndFieldTypeIsDate_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Date Field",
      Type = FieldType.Date,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var mockPropertyInfo = new Mock<PropertyInfo>();
    mockPropertyInfo
      .SetupGet(static x => x.PropertyType)
      .Returns(typeof(DateTime?));

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      mockPropertyInfo.Object
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsDateTimeAndFieldTypeIsText_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Text Field",
      Type = FieldType.Text,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var mockPropertyInfo = new Mock<PropertyInfo>();
    mockPropertyInfo
      .SetupGet(static x => x.PropertyType)
      .Returns(typeof(DateTime?));

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      mockPropertyInfo.Object
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsDateTimeAndFieldTypeIsNotDateOrText_ItShouldReturnFalse()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var mockPropertyInfo = new Mock<PropertyInfo>();
    mockPropertyInfo
      .SetupGet(static x => x.PropertyType)
      .Returns(typeof(DateTime?));

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      mockPropertyInfo.Object
    );

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsDateTimeOffsetAndFieldTypeIsDate_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Date Field",
      Type = FieldType.Date,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(User).GetProperty("CreatedDateTime");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsDateTimeOffsetAndFieldTypeIsText_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Text Field",
      Type = FieldType.Text,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(User).GetProperty("CreatedDateTime");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsDateTimeOffsetAndFieldTypeIsNotDateOrText_ItShouldReturnFalse()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(User).GetProperty("CreatedDateTime");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsAListOfStringsAndFieldTypeIsMultiSelectList_ItShouldReturnTrue()
  {
    var onspringField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Multiplicity = Multiplicity.MultiSelect,
    };

    var azurePropertyInfo = typeof(Group).GetProperty("GroupTypes");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsAListOfStringsAndFieldTypeIsText_ItShouldReturnTrue()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Text Field",
      Type = FieldType.Text,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(Group).GetProperty("GroupTypes");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsAListOfStringsAndFieldTypeIsSingleSelectList_ItShouldReturnFalse()
  {
    var onspringField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Multiplicity = Multiplicity.SingleSelect,
    };

    var azurePropertyInfo = typeof(Group).GetProperty("GroupTypes");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsAListOfStringsAndFieldTypeIsNotMultiSelectListOrText_ItShouldReturnFalse()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Date Field",
      Type = FieldType.Date,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(Group).GetProperty("GroupTypes");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidFieldTypeAndPropertyType_WhenCalledAndPropertyTypeIsNotSupported_ItShouldReturnFalse()
  {
    var onspringField = new Field
    {
      Id = 1,
      AppId = 1,
      Name = "Text Field",
      Type = FieldType.Text,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
    };

    var azurePropertyInfo = typeof(Group).GetProperty("AssignedLabels");

    var result = Processor.IsValidFieldTypeAndPropertyType(
      onspringField,
      azurePropertyInfo!
    );

    result.Should().BeFalse();
  }

  [Fact]
  public void HasValidFieldTypeToPropertyTypeMappings_WhenCalledAndFieldHasValidFieldTypeToPropertyTypeMappings_ItShouldReturnTrue()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    var groupsFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
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

    var usersFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      }
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = groupsFields,
        UsersFields = usersFields
      });

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "DisplayName" },
        { 2, "Description" },
        { -1, "invalid" },
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "UserPrincipalName" },
        { 2, "GivenName" },
        { 3, "Surname" },
        { 4, "Mail" },
        { -1, "invalid" },
      });

    var result = _processor.HasValidFieldTypeToPropertyTypeMappings();

    result.Should().BeTrue();
  }

  [Fact]
  public void HasValidFieldTypeToPropertyTypeMappings_WhenCalledAndFieldHasInvalidFieldTypeToPropertyTypeMappings_ItShouldReturnFalse()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    var groupsFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "Description",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
    };

    var usersFields = new List<Field>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "Username",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "First Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "Last Name",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 4,
        AppId = 1,
        Name = "Email Address",
        Type = FieldType.Text,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
      new()
      {
        Id = 5,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
      },
    };

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = groupsFields,
        UsersFields = usersFields
      });

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "DisplayName" },
        { 2, "Description" },
        { 0, "invalid" },
        { 3, "createdDateTime" }
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns(new Dictionary<int, string>
      {
        { 1, "UserPrincipalName" },
        { 2, "GivenName" },
        { 3, "Surname" },
        { 4, "Mail" },
        { 0, "invalid" },
        { 5, "createdDateTime" }
      });

    var result = _processor.HasValidFieldTypeToPropertyTypeMappings();

    result.Should().BeFalse();
  }

  [Fact]
  public void TryGetNewListValue_WhenCalledAndValueIsNotInList_ItShouldTrueAndSetNewListValueToDictionaryOfListIdAndNewValueName()
  {
    var listId = 1;

    var listField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Multiplicity = Multiplicity.SingleSelect,
      Values = [],
      ListId = listId,
    };

    var possibleNewListValue = "new value";

    var result = Processor.TryGetNewListValue(listField, possibleNewListValue, out var newListValue);

    result.Should().BeTrue();
    newListValue.Should().BeOfType<KeyValuePair<int, string>>();
    newListValue.Key.Should().Be(listId);
    newListValue.Value.Should().Be(possibleNewListValue);
  }

  [Fact]
  public void TryGetNewListValue_WhenCalledAndValueIsInList_ItShouldReturnFalseAndSetNewListValueToNull()
  {
    var listId = 1;
    var existingValue = "existing value";

    var listField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Multiplicity = Multiplicity.SingleSelect,
      Values = [
        new ListValue
        {
          Id = Guid.NewGuid(),
          Name = existingValue
        }
      ],
      ListId = listId,
    };

    var result = Processor.TryGetNewListValue(listField, existingValue, out var newListValue);

    result.Should().BeFalse();
    newListValue.Value.Should().BeNull();
  }

  [Fact]
  public void TryGetNewListValue_WhenCalledAndValueIsNull_ItShouldReturnFalseAndSetNewListValue()
  {
    var listField = new ListField
    {
      Id = 1,
      AppId = 1,
      Name = "List Field",
      Type = FieldType.List,
      Status = FieldStatus.Enabled,
      IsRequired = true,
      IsUnique = true,
      Multiplicity = Multiplicity.SingleSelect,
      Values = [
        new ListValue
        {
          Id = Guid.NewGuid(),
          Name = "name"
        }
      ],
      ListId = 1,
    };

    var result = Processor.TryGetNewListValue(listField, null, out var newListValue);

    result.Should().BeFalse();
    newListValue.Value.Should().BeNull();
  }

  [Fact]
  public void TryGetNewListValue_WhenCalledAndFieldIsNull_ItShouldReturnFalseAndSetNewListValue()
  {
    var result = Processor.TryGetNewListValue(null, "some value", out var newListValue);

    result.Should().BeFalse();
    newListValue.Value.Should().BeNull();
  }

  [Fact]
  public async Task SyncListValues_WhenCalledAndNewValuesAreFoundForGroupsListFields_ItShouldAddTheListValuesAndUpdateGroupFields()
  {
    var onspringSettings = new OnspringSettings();

    _settingsMock
    .SetupGet(static x => x.Onspring)
    .Returns(onspringSettings);

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    var listFields = new List<ListField>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.MultiSelect,
        ListId = 1,
        Values = [
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "group type 1"
          }
        ],
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 2,
        Values = [],
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 3,
        Values = [],
      },
    };

    var fieldMappings = new Dictionary<int, string>
    {
      { 1, "groupTypes" },
      { 2, "displayName" },
      { 3, "invalidProperty" },
    };

    var groups = new List<Group?>
    {
      new()
      {
        Id = "group id",
        DisplayName = "group display name",
        Description = "group description",
        GroupTypes = [
          "group type 1",
          "group type 2",
        ]
      },
      null,
      new()
      {
        Id = "group id",
        DisplayName = null,
        Description = "group description",
        GroupTypes = null,
      },
      new()
      {
        Id = "group id",
        DisplayName = "group display name",
        Description = "group description",
        GroupTypes = [],
      },
    };

    var updatedListFields = new List<Field>
    {
      new ListField
      {
        Id = 1,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.MultiSelect,
        ListId = 1,
        Values = [
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "group type 1"
          },
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "group type 2"
          }
        ],
      },
      new ListField
      {
        Id = 2,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 2,
        Values = [
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "group display name"
          }
        ],
      },
    };

    _onspringServiceMock
      .Setup(static x => x.AddListValue(It.IsAny<int>(), It.IsAny<string>()))
      .ReturnsAsync(new SaveListItemResponse(Guid.NewGuid()));

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync(updatedListFields);

    await _processor.SyncListValues(
      listFields,
      fieldMappings,
      groups
    );

    _onspringServiceMock.Verify(
      static x => x.AddListValue(
        It.IsAny<int>(),
        It.IsAny<string>()
      ),
      Times.Exactly(2)
    );

    _onspringServiceMock.Verify(static x => x.GetGroupFields(), Times.Once);

    onspringSettings.GroupsFields.Should().BeEquivalentTo(updatedListFields);
  }

  [Fact]
  public async Task SyncListValues_WhenCalledAndNewValuesAreFoundForUsersListFields_ItShouldAddTheListValuesAndUpdateUserFields()
  {
    var onspringSettings = new OnspringSettings();

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(onspringSettings);

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    var listFields = new List<ListField>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.MultiSelect,
        ListId = 1,
        Values = [
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "LastName"
          }
        ],
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 2,
        Values = [],
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 2,
        Values = [],
      },
    };

    var fieldMappings = new Dictionary<int, string>
    {
      { 1, "surname" },
      { 2, "accountEnabled" },
      { 3, "invalidProperty"},
    };

    var users = new List<User?>
    {
      new()
      {
        Id = "user id",
        Surname = "NewName",
        AccountEnabled = true,
      },
      null,
      new()
      {
        Id = "user id",
        Surname = "LastName",
        AccountEnabled = null,
      },
    };

    var updatedListFields = new List<Field>
    {
      new ListField
      {
        Id = 1,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.MultiSelect,
        ListId = 1,
        Values = [
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "LastName"
          },
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "NewName"
          }
        ],
      },
      new ListField
      {
        Id = 2,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 2,
        Values = [
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "true"
          }
        ],
      },
    };

    _onspringServiceMock
      .Setup(static x => x.AddListValue(It.IsAny<int>(), It.IsAny<string>()))
      .ReturnsAsync(new SaveListItemResponse(Guid.NewGuid()));

    _onspringServiceMock
      .Setup(static x => x.GetUserFields())
      .ReturnsAsync(updatedListFields);

    await _processor.SyncListValues(
      listFields,
      fieldMappings,
      users
    );

    _onspringServiceMock.Verify(
      static x => x.AddListValue(
        It.IsAny<int>(),
        It.IsAny<string>()
      ),
      Times.Exactly(2)
    );

    _onspringServiceMock.Verify(static x => x.GetUserFields(), Times.Once);

    onspringSettings.UsersFields.Should().BeEquivalentTo(updatedListFields);
  }


  [Fact]
  public async Task SyncListValues_WhenCalledAndNewValuesAreFoundForListFieldsAndAListValueCanNotBeAdded_ItShouldLogAWarning()
  {
    var onspringSettings = new OnspringSettings();

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(onspringSettings);

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    var listFields = new List<ListField>
    {
      new()
      {
        Id = 1,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.MultiSelect,
        ListId = 1,
        Values = [
          new ListValue
          {
            Id = Guid.NewGuid(),
            Name = "LastName"
          }
        ],
      },
      new()
      {
        Id = 2,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 2,
        Values = [],
      },
      new()
      {
        Id = 3,
        AppId = 1,
        Name = "List Field",
        Type = FieldType.List,
        Status = FieldStatus.Enabled,
        IsRequired = true,
        IsUnique = true,
        Multiplicity = Multiplicity.SingleSelect,
        ListId = 2,
        Values = [],
      },
    };

    var fieldMappings = new Dictionary<int, string>
    {
      { 1, "surname" },
      { 2, "accountEnabled" },
      { 3, "invalidProperty"},
    };

    var users = new List<User?>
    {
      new()
      {
        Id = "user id",
        Surname = "NewName",
        AccountEnabled = true,
      },
      null,
      new() {
        Id = "user id",
        Surname = "LastName",
        AccountEnabled = null,
      },
    };

    _onspringServiceMock
      .Setup(static x => x.AddListValue(It.IsAny<int>(), It.IsAny<string>()))
      .ReturnsAsync(null as SaveListItemResponse);

    await _processor.SyncListValues(
      listFields,
      fieldMappings,
      users
    );

    _onspringServiceMock.Verify(static x => x.AddListValue(It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(2));

    _onspringServiceMock.Verify(static x => x.GetUserFields(), Times.Once);

    _loggerMock.Verify(
      static x => x.Warning(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<int>()
      ),
      Times.Exactly(2)
    );
  }

  [Fact]
  public async Task SyncGroups_WhenCalledAndNoGroupsAreFound_ItShouldNotSyncGroupsToOnspring()
  {
    var groups = new List<Group>();

    _graphServiceMock
      .Setup(static x => x.GetGroupsIterator(It.IsAny<List<Group>>(), It.IsAny<int>()))
      .ReturnsAsync(null as PageIterator<Group, GroupCollectionResponse>);

    await _processor.SyncGroups();

    _graphServiceMock.Verify(static x => x.GetGroupsIterator(It.IsAny<List<Group>>(), It.IsAny<int>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.GetGroup(It.IsAny<Group>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.CreateGroup(It.IsAny<Group>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()), Times.Never);
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
      new()
      {
        Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
        Description = "Test Group 1",
      },
      new()
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
      FieldData = [
        new StringFieldValue(1, "1f01a3d4-7142-4210-b54d-9aadf98ce929"),
        new StringFieldValue(2, "Needs updated")
      ]
    };

    // setup msGraph that we can mock
    // to return initial set of groups
    // to create page iterator
    var msGraphMock = new Mock<IMsGraph>();

    // mock to return collection of groups
    msGraphMock
      .Setup(static x => x.GetGroupsForIterator(It.IsAny<Dictionary<int, string>>(), string.Empty))
      .ReturnsAsync(azureGroupCollection);

    // mock graph service client for msGraphMock
    var tokenCredentialMock = new Mock<TokenCredential>();
    msGraphMock
      .SetupGet(static x => x.GraphServiceClient)
      .Returns(new GraphServiceClient(
        tokenCredentialMock.Object,
        null,
        null
      ));

    // create new graph service using 
    // the test specific msGraphMock
    var graphService = new GraphService(
      _loggerMock.Object,
      _settingsMock.Object,
      msGraphMock.Object
    );

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        GroupsFields = [
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
          new ListField
          {
            Id = 2,
            AppId = 1,
            Name = "Description",
            Type = FieldType.List,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
            Values = [
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Needs updated"
              },
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Updated"
              }
            ]
          }
        ]
      }
    );

    _settingsMock
      .SetupGet(static x => x.GroupsFieldMappings)
      .Returns([]);

    // setup onspring service
    // to mock returning nul for first
    // azure group and a found onspring
    // group for the second azure group
    _onspringServiceMock
      .SetupSequence(static x => x.GetGroup(It.IsAny<Group>()))
      .ReturnsAsync(null as ResultRecord)
      .ReturnsAsync(onspringGroup);

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

    _onspringServiceMock.Verify(static x => x.GetGroup(It.IsAny<Group>()), Times.Exactly(2));
    _onspringServiceMock.Verify(static x => x.CreateGroup(It.IsAny<Group>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()), Times.Once);
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
      Warnings = []
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroup(It.IsAny<Group>()))
      .ReturnsAsync(null as ResultRecord);

    _onspringServiceMock
      .Setup(static x => x.CreateGroup(It.IsAny<Group>()))
      .ReturnsAsync(saveRecordResponse);

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(static x => x.CreateGroup(It.IsAny<Group>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()), Times.Never);
  }

  [Fact]
  public async Task SyncGroup_WhenCalledAndGroupIsNotFoundInOnspringButGroupIsNotCreatedInOnspring_ItShouldLogWarning()
  {
    var azureGroup = new Group
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      Description = "Test Group 1",
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroup(It.IsAny<Group>()))
      .ReturnsAsync(null as ResultRecord);

    _onspringServiceMock
    .Setup(static x => x.CreateGroup(It.IsAny<Group>()))
    .ReturnsAsync(null as SaveRecordResponse);

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(static x => x.CreateGroup(It.IsAny<Group>()), Times.Once);

    _loggerMock.Verify(static x => x.Warning(It.IsAny<string>(), It.IsAny<Group>()), Times.Once);

    _onspringServiceMock.Verify(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()), Times.Never);
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
      FieldData = [
        new StringFieldValue(1, "98e58dab-9f2c-4216-bc91-70d7dabe227e"),
        new StringFieldValue(2, "Group that needs updating"),
      ]
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = []
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroup(It.IsAny<Group>()))
      .ReturnsAsync(resultRecord);

    _onspringServiceMock
      .Setup(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()))
      .ReturnsAsync(saveRecordResponse);

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(static x => x.CreateGroup(It.IsAny<Group>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()), Times.Once);
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
      FieldData = [
        new StringFieldValue(1, "98e58dab-9f2c-4216-bc91-70d7dabe227e"),
        new StringFieldValue(2, "Group that needs updating"),
      ]
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroup(It.IsAny<Group>()))
      .ReturnsAsync(resultRecord);

    _onspringServiceMock
      .Setup(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()))
      .ReturnsAsync(null as SaveRecordResponse);

    await _processor.SyncGroup(azureGroup);

    _onspringServiceMock.Verify(static x => x.CreateGroup(It.IsAny<Group>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateGroup(It.IsAny<Group>(), It.IsAny<ResultRecord>()), Times.Once);
    _loggerMock.Verify(static x => x.Warning(It.IsAny<string>(), It.IsAny<ResultRecord>()), Times.Once);
  }

  [Fact]
  public async Task SyncUsers_WhenCalledAndNoUsersAreFound_ItShouldNotSyncUsersToOnspring()
  {
    var groups = new List<Group>();

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _graphServiceMock
      .Setup(static x => x.GetUsersIterator(It.IsAny<List<User>>(), It.IsAny<int>()))
      .ReturnsAsync(null as PageIterator<User, UserCollectionResponse>);

    await _processor.SyncUsers();

    _graphServiceMock.Verify(static x => x.GetUsersIterator(It.IsAny<List<User>>(), It.IsAny<int>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
  }

  // Note: This test is complicated by the fact
  // we are using the page iterator class to iterator over users
  // and then also using an wrapper class on the actual
  // graphServiceClient to make unit testing possible
  [Fact]
  public async Task SyncUsers_WhenCalledAndUsersAreFoundAndNoUserGroupsAreFound_ItShouldSyncUsersToOnspring()
  {
    var azureUsers = new List<User>
    {
      new()
      {
        Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
        UserPrincipalName = "User1",
        GivenName = "User",
        Surname = "One",
        Mail = "user.one@test.com",
        AccountEnabled = true,
      },
      new()
      {
        Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
        UserPrincipalName = "User2",
        GivenName = "User",
        Surname = "Two",
        Mail = "user.two@test.com",
        AccountEnabled = true,
      },
    };

    // setup azure users collection to return
    // as initial users when creating page iterator
    var azureUsersCollection = new UserCollectionResponse
    {
      Value = azureUsers
    };

    // setup onspring user that we will
    // pretend was found by onspring service
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = [
        new StringFieldValue(1, "User2"),
        new StringFieldValue(2, "User"),
        new StringFieldValue(3, "Two"),
        new StringFieldValue(4, "user.two@test.com"),
        new StringFieldValue(5, "Active"),
      ]
    };

    // setup msGraph that we can mock
    // to return initial set of users
    // to create page iterator
    var msGraphMock = new Mock<IMsGraph>();

    // mock to return collection of users
    msGraphMock
      .Setup(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()))
      .ReturnsAsync(azureUsersCollection);

    // mock graph service client for msGraphMock
    var tokenCredentialMock = new Mock<TokenCredential>();

    msGraphMock
      .SetupGet(static x => x.GraphServiceClient)
      .Returns(new GraphServiceClient(
        tokenCredentialMock.Object,
        null,
        null
      ));

    // create new graph service using 
    // the test specific msGraphMock
    var graphService = new GraphService(
      _loggerMock.Object,
      _settingsMock.Object,
      msGraphMock.Object
    );

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .Returns<List<DirectoryObject>>(null!);

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        UsersFields = [
          new Field
          {
            Id = 1,
            AppId = 1,
            Name = "Username",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 2,
            AppId = 1,
            Name = "First Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 3,
            AppId = 1,
            Name = "Last Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 4,
            AppId = 1,
            Name = "Email Address",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new ListField
          {
            Id = 5,
            AppId = 1,
            Name = "Status",
            Type = FieldType.List,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
            Values = [
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Active"
              },
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Inactive"
              }
            ]
          }
        ]
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns([]);

    // setup onspring service
    // to mock returning null for first
    // azure user and a found onspring
    // user for the second azure user
    _onspringServiceMock
      .SetupSequence(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(null as ResultRecord)
      .ReturnsAsync(onspringUser);

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    // create new instance of processor
    // for this specific test to use
    // the test specific graph service
    var processor = new Processor(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringServiceMock.Object,
      graphService
    );

    await processor.SyncUsers();

    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Exactly(2));
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
  }

  // Note: This test is complicated by the fact
  // we are using the page iterator class to iterator over users
  // and then also using an wrapper class on the actual
  // graphServiceClient to make unit testing possible
  [Fact]
  public async Task SyncUsers_WhenCalledAndUsersAreFoundAndUserGroupsAreFound_ItShouldSyncUsersToOnspring()
  {
    var azureUsers = new List<User>
    {
      new()
      {
        Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
        UserPrincipalName = "User1",
        GivenName = "User",
        Surname = "One",
        Mail = "user.one@test.com",
        AccountEnabled = true,
      },
      new()
      {
        Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
        UserPrincipalName = "User2",
        GivenName = "User",
        Surname = "Two",
        Mail = "user.two@test.com",
        AccountEnabled = true,
      },
    };

    // setup azure users collection to return
    // as initial users when creating page iterator
    var azureUsersCollection = new UserCollectionResponse
    {
      Value = azureUsers
    };

    // setup onspring user that we will
    // pretend was found by onspring service
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = [
        new StringFieldValue(1, "User2"),
        new StringFieldValue(2, "User"),
        new StringFieldValue(3, "Two"),
        new StringFieldValue(4, "user.two@test.com"),
        new StringFieldValue(5, "Active"),
      ]
    };

    // setup msGraph that we can mock
    // to return initial set of users
    // to create page iterator
    var msGraphMock = new Mock<IMsGraph>();

    // mock to return collection of users
    msGraphMock
      .Setup(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()))
      .ReturnsAsync(azureUsersCollection);

    // mock graph service client for msGraphMock
    var tokenCredentialMock = new Mock<TokenCredential>();

    msGraphMock
      .SetupGet(static x => x.GraphServiceClient)
      .Returns(new GraphServiceClient(
        tokenCredentialMock.Object,
        null,
        null
      ));

    // create new graph service using 
    // the test specific msGraphMock
    var graphService = new GraphService(
      _loggerMock.Object,
      _settingsMock.Object,
      msGraphMock.Object
    );

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .ReturnsAsync([
        new Group
        {
          Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
          Description = "Group 1 Description",
        },
        new Group
        {
          Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
          Description = "Group 2 Description",
        },
      ]);

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        UsersFields = [
          new Field
          {
            Id = 1,
            AppId = 1,
            Name = "Username",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 2,
            AppId = 1,
            Name = "First Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 3,
            AppId = 1,
            Name = "Last Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 4,
            AppId = 1,
            Name = "Email Address",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new ListField
          {
            Id = 5,
            AppId = 1,
            Name = "Status",
            Type = FieldType.List,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
            Values = [
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Active"
              },
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Inactive"
              }
            ]
          }
        ]
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns([]);

    // setup onspring service
    // to mock returning null for first
    // azure user and a found onspring
    // user for the second azure user
    _onspringServiceMock
      .SetupSequence(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(null as ResultRecord)
      .ReturnsAsync(onspringUser);

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    // create new instance of processor
    // for this specific test to use
    // the test specific graph service
    var processor = new Processor(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringServiceMock.Object,
      graphService
    );

    await processor.SyncUsers();

    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Exactly(2));
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
  }

  [Fact]
  public async Task SyncUser_WhenCalledAndUserIsNotFound_ItShouldCreateUserInOnspring()
  {
    var azureUser = new User
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      UserPrincipalName = "User2",
      GivenName = "User",
      Surname = "Two",
      Mail = "user.two@test.com",
      AccountEnabled = true,
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = []
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .ReturnsAsync([]);

    _onspringServiceMock
      .Setup(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(null as ResultRecord);

    _onspringServiceMock
      .Setup(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()))
      .ReturnsAsync(saveRecordResponse);

    await _processor.SyncUser(azureUser);

    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
  }

  [Fact]
  public async Task SyncUser_WhenCalledAndUserIsNotFoundInOnspringButUserIsNotCreatedInOnspring_ItShouldLogWarning()
  {
    var azureUser = new User
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      UserPrincipalName = "User2",
      GivenName = "User",
      Surname = "Two",
      Mail = "user.two@test.com",
      AccountEnabled = true,
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = []
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .ReturnsAsync([
        new Group
        {
          Id = "1",
          DisplayName = "Group 1",
        },
      ]);

    _onspringServiceMock
      .Setup(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(null as ResultRecord);

    _onspringServiceMock
      .Setup(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()))
      .ReturnsAsync(null as SaveRecordResponse);

    await _processor.SyncUser(azureUser);

    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
    _loggerMock.Verify(static x => x.Warning(It.IsAny<string>(), It.IsAny<User>()), Times.Once);
  }

  [Fact]
  public async Task SyncUser_WhenCalledAndUserIsFoundInOnspring_ItShouldUpdateUserInOnspring()
  {
    var azureUser = new User
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      UserPrincipalName = "User2",
      GivenName = "User",
      Surname = "Two",
      Mail = "user.two@test.com",
      AccountEnabled = true,
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = []
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .ReturnsAsync([]);

    _onspringServiceMock
      .Setup(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(new ResultRecord
      {
        AppId = 1,
        RecordId = 1,
        FieldData = [],
      });

    _onspringServiceMock
      .Setup(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()))
      .ReturnsAsync(saveRecordResponse);

    await _processor.SyncUser(azureUser);

    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
  }

  [Fact]
  public async Task SyncUser_WhenCalledAndUserIsFoundInOnspringAndUserIsNotUpdated_ItShouldLogWarning()
  {
    var azureUser = new User
    {
      Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
      UserPrincipalName = "User2",
      GivenName = "User",
      Surname = "Two",
      Mail = "user.two@test.com",
      AccountEnabled = true,
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = []
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .ReturnsAsync([
        new Group
        {
          Id = "1",
          DisplayName = "Group 1",
        },
      ]);

    _onspringServiceMock
      .Setup(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(new ResultRecord
      {
        AppId = 1,
        RecordId = 1,
        FieldData = [],
      });

    _onspringServiceMock
      .Setup(
        static x => x.UpdateUser(
          It.IsAny<User>(),
          It.IsAny<ResultRecord>(),
          It.IsAny<Dictionary<string, int>>()
        )
      )
      .ReturnsAsync(null as SaveRecordResponse);

    await _processor.SyncUser(azureUser);

    _onspringServiceMock.Verify(
      static x => x.CreateUser(
        It.IsAny<User>(),
        It.IsAny<Dictionary<string, int>>()
      ),
      Times.Never
    );

    _onspringServiceMock.Verify(
      static x => x.UpdateUser(
        It.IsAny<User>(),
        It.IsAny<ResultRecord>(),
        It.IsAny<Dictionary<string, int>>()
      ),
      Times.Once
    );

    _loggerMock.Verify(static x => x.Warning(It.IsAny<string>(), It.IsAny<ResultRecord>()), Times.Once);
  }

  [Fact]
  public async Task HasValidGroupFilters_WhenCalledAndNoGroupFiltersArePresent_ItShouldReturnTrue()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _graphServiceMock
      .Setup(static x => x.CanGetGroups(It.IsAny<string>()))
      .ReturnsAsync((true, string.Empty));

    var (isSuccessful, _) = await _processor.HasValidGroupFilter();

    isSuccessful.Should().BeTrue();
  }

  [Fact]
  public async Task HasValidGroupFilters_WhenCalledAndGroupFiltersArePresent_ItShouldReturnTrue()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings
      {
        GroupFilter = "displayName eq 'Test Group'"
      });

    _graphServiceMock
      .Setup(static x => x.CanGetGroups(It.IsAny<string>()))
      .ReturnsAsync((true, string.Empty));

    var (isSuccesful, _) = await _processor.HasValidGroupFilter();

    isSuccesful.Should().BeTrue();
  }

  [Fact]
  public async Task HasValidGroupFilters_WhenCalledAndGroupFiltersHasInvalidFilter_ItShouldReturnFalse()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings
      {
        GroupFilter = "invalid filter"
      });

    _graphServiceMock
      .Setup(static x => x.CanGetGroups(It.IsAny<string>()))
      .ReturnsAsync((false, "Invalid filter"));

    var (isSuccesful, _) = await _processor.HasValidGroupFilter();

    isSuccesful.Should().BeFalse();
  }

  [Fact]
  public async Task SyncGroupMembers_WhenCalledAndGroupIsNull_ItShouldNotSyncMembersToOnspring()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _graphServiceMock
      .Setup(static x => x.GetGroupMembersIterator(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<int>()))
      .ReturnsAsync(null as PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>);

    await _processor.SyncGroupMembers(null!);

    _graphServiceMock.Verify(static x => x.GetGroupMembersIterator(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<int>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
  }

  [Fact]
  public async Task SyncGroupMembers_WhenCalledAndGroupIdIsNull_ItShouldNotSyncMembersToOnspring()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _graphServiceMock
      .Setup(static x => x.GetGroupMembersIterator(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<int>()))
      .ReturnsAsync(null as PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>);

    await _processor.SyncGroupMembers(new Group());

    _graphServiceMock.Verify(static x => x.GetGroupMembersIterator(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<int>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
  }

  [Fact]
  public async Task SyncGroupMembers_WhenCalledAndNoMembersAreFound_ItShouldNotSyncMembersToOnspring()
  {
    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _graphServiceMock
      .Setup(static x => x.GetGroupMembersIterator(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<int>()))
      .ReturnsAsync(null as PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>);

    await _processor.SyncGroupMembers(new Group() { Id = Guid.NewGuid().ToString() });

    _graphServiceMock.Verify(static x => x.GetGroupMembersIterator(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<int>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Never);
  }

  // Note: This test is complicated by the fact
  // we are using the page iterator class to iterator over members
  // and then also using an wrapper class on the actual
  // graphServiceClient to make unit testing possible
  [Fact]
  public async Task SyncUsers_WhenCalledAndMembersAreFoundAndNoUserGroupsAreFound_ItShouldSyncMembersToOnspring()
  {
    var azureUsers = new List<DirectoryObject>
    {
      new User()
      {
        Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
        UserPrincipalName = "User1",
        GivenName = "User",
        Surname = "One",
        Mail = "user.one@test.com",
        AccountEnabled = true,
      },
      new User()
      {
        Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
        UserPrincipalName = "User2",
        GivenName = "User",
        Surname = "Two",
        Mail = "user.two@test.com",
        AccountEnabled = true,
      },
    };

    // setup azure users collection to return
    // as initial users when creating page iterator
    var azureUsersCollection = new DirectoryObjectCollectionResponse
    {
      Value = azureUsers
    };

    // setup onspring user that we will
    // pretend was found by onspring service
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = [
        new StringFieldValue(1, "User2"),
        new StringFieldValue(2, "User"),
        new StringFieldValue(3, "Two"),
        new StringFieldValue(4, "user.two@test.com"),
        new StringFieldValue(5, "Active"),
      ]
    };

    // setup msGraph that we can mock
    // to return initial set of users
    // to create page iterator
    var msGraphMock = new Mock<IMsGraph>();

    // mock to return collection of users
    msGraphMock
      .Setup(static x => x.GetGroupMembersForIterator(It.IsAny<string>(), It.IsAny<List<string>>()))
      .ReturnsAsync(azureUsersCollection);

    // mock graph service client for msGraphMock
    var tokenCredentialMock = new Mock<TokenCredential>();

    msGraphMock
      .SetupGet(static x => x.GraphServiceClient)
      .Returns(new GraphServiceClient(
        tokenCredentialMock.Object,
        null,
        null
      ));

    // create new graph service using 
    // the test specific msGraphMock
    var graphService = new GraphService(
      _loggerMock.Object,
      _settingsMock.Object,
      msGraphMock.Object
    );

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .Returns<List<DirectoryObject>>(null!);

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        UsersFields = [
          new Field
          {
            Id = 1,
            AppId = 1,
            Name = "Username",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 2,
            AppId = 1,
            Name = "First Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 3,
            AppId = 1,
            Name = "Last Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 4,
            AppId = 1,
            Name = "Email Address",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new ListField
          {
            Id = 5,
            AppId = 1,
            Name = "Status",
            Type = FieldType.List,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
            Values = [
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Active"
              },
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Inactive"
              }
            ]
          }
        ]
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns([]);

    // setup onspring service
    // to mock returning null for first
    // azure user and a found onspring
    // user for the second azure user
    _onspringServiceMock
      .SetupSequence(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(null as ResultRecord)
      .ReturnsAsync(onspringUser);

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    // create new instance of processor
    // for this specific test to use
    // the test specific graph service
    var processor = new Processor(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringServiceMock.Object,
      graphService
    );

    await processor.SyncGroupMembers(new Group() { Id = Guid.NewGuid().ToString() });

    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Exactly(2));
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
  }

  // Note: This test is complicated by the fact
  // we are using the page iterator class to iterator over members
  // and then also using an wrapper class on the actual
  // graphServiceClient to make unit testing possible
  [Fact]
  public async Task SyncUsers_WhenCalledAndMembersAreFoundAndUserGroupsAreFound_ItShouldSyncMembersToOnspring()
  {
    var azureUsers = new List<DirectoryObject>
    {
      new User()
      {
        Id = "98e58dab-9f2c-4216-bc91-70d7dabe227e",
        UserPrincipalName = "User1",
        GivenName = "User",
        Surname = "One",
        Mail = "user.one@test.com",
        AccountEnabled = true,
      },
      new User()
      {
        Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
        UserPrincipalName = "User2",
        GivenName = "User",
        Surname = "Two",
        Mail = "user.two@test.com",
        AccountEnabled = true,
      },
    };

    // setup azure users collection to return
    // as initial users when creating page iterator
    var azureUsersCollection = new DirectoryObjectCollectionResponse
    {
      Value = azureUsers
    };

    // setup onspring user that we will
    // pretend was found by onspring service
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = [
        new StringFieldValue(1, "User2"),
        new StringFieldValue(2, "User"),
        new StringFieldValue(3, "Two"),
        new StringFieldValue(4, "user.two@test.com"),
        new StringFieldValue(5, "Active"),
      ]
    };

    // setup msGraph that we can mock
    // to return initial set of users
    // to create page iterator
    var msGraphMock = new Mock<IMsGraph>();

    // mock to return collection of users
    msGraphMock
      .Setup(static x => x.GetGroupMembersForIterator(It.IsAny<string>(), It.IsAny<List<string>>()))
      .ReturnsAsync(azureUsersCollection);

    // mock graph service client for msGraphMock
    var tokenCredentialMock = new Mock<TokenCredential>();

    msGraphMock
      .SetupGet(static x => x.GraphServiceClient)
      .Returns(new GraphServiceClient(
        tokenCredentialMock.Object,
        null,
        null
      ));

    // create new graph service using 
    // the test specific msGraphMock
    var graphService = new GraphService(
      _loggerMock.Object,
      _settingsMock.Object,
      msGraphMock.Object
    );

    _graphServiceMock
      .Setup(static x => x.GetUserGroups(It.IsAny<User>()))
      .ReturnsAsync([
        new Group
        {
          Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
          Description = "Group 1 Description",
        },
        new Group
        {
          Id = "1f01a3d4-7142-4210-b54d-9aadf98ce929",
          Description = "Group 2 Description",
        },
      ]);

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _settingsMock
      .SetupGet(static x => x.Onspring)
      .Returns(new OnspringSettings
      {
        UsersFields = [
          new Field
          {
            Id = 1,
            AppId = 1,
            Name = "Username",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new Field
          {
            Id = 2,
            AppId = 1,
            Name = "First Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 3,
            AppId = 1,
            Name = "Last Name",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = false,
          },
          new Field
          {
            Id = 4,
            AppId = 1,
            Name = "Email Address",
            Type = FieldType.Text,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
          },
          new ListField
          {
            Id = 5,
            AppId = 1,
            Name = "Status",
            Type = FieldType.List,
            Status = FieldStatus.Enabled,
            IsRequired = true,
            IsUnique = true,
            Values = [
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Active"
              },
              new ListValue
              {
                Id = Guid.NewGuid(),
                Name = "Inactive"
              }
            ]
          }
        ]
      });

    _settingsMock
      .SetupGet(static x => x.UsersFieldMappings)
      .Returns([]);

    // setup onspring service
    // to mock returning null for first
    // azure user and a found onspring
    // user for the second azure user
    _onspringServiceMock
      .SetupSequence(static x => x.GetUser(It.IsAny<User>()))
      .ReturnsAsync(null as ResultRecord)
      .ReturnsAsync(onspringUser);

    _onspringServiceMock
      .Setup(static x => x.GetGroupFields())
      .ReturnsAsync([
        new Field
        {
          Id = 1,
          AppId = 1,
          Name = "Record Id",
          Type = FieldType.AutoNumber,
          Status = FieldStatus.Enabled,
          IsRequired = true,
          IsUnique = true,
        },
      ]);

    // create new instance of processor
    // for this specific test to use
    // the test specific graph service
    var processor = new Processor(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringServiceMock.Object,
      graphService
    );

    await processor.SyncGroupMembers(new Group() { Id = Guid.NewGuid().ToString() });

    _onspringServiceMock.Verify(static x => x.GetUser(It.IsAny<User>()), Times.Exactly(2));
    _onspringServiceMock.Verify(static x => x.CreateUser(It.IsAny<User>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
    _onspringServiceMock.Verify(static x => x.UpdateUser(It.IsAny<User>(), It.IsAny<ResultRecord>(), It.IsAny<Dictionary<string, int>>()), Times.Once);
  }
}