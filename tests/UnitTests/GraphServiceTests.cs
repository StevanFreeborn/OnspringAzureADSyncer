namespace OnspringAzureADSyncerTests.UnitTests;

public class GraphServiceTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<ISettings> _settingsMock;
  private readonly Mock<IMsGraph> _msGraphMock;

  private readonly GraphService _graphService;

  public GraphServiceTests()
  {
    _loggerMock = new Mock<ILogger>();
    _settingsMock = new Mock<ISettings>();
    _msGraphMock = new Mock<IMsGraph>();

    var tokenCredentialMock = new Mock<TokenCredential>();

    _msGraphMock
      .SetupGet(static x => x.GraphServiceClient)
      .Returns(new GraphServiceClient(tokenCredentialMock.Object, null, null));

    _graphService = new GraphService(
      _loggerMock.Object,
      _settingsMock.Object,
      _msGraphMock.Object
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndCanGetUsers_ItSHouldReturnTrue()
  {
    var usersCollection = new UserCollectionResponse();

    _msGraphMock
      .Setup(static x => x.GetUsers())
      .ReturnsAsync(usersCollection);

    var result = await _graphService.CanGetUsers();

    result.Should().BeTrue();
    _msGraphMock.Verify(static x => x.GetUsers(), Times.Once);
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndCannotGetUsers_ItShouldReturnFalse()
  {
    _msGraphMock
      .Setup(static x => x.GetUsers())
      .Throws(new Exception());

    var result = await _graphService.CanGetUsers();

    result.Should().BeFalse();
    _msGraphMock.Verify(static x => x.GetUsers(), Times.Once);
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndCanGetGroups_ItShouldReturnTrue()
  {
    var groupsCollection = new GroupCollectionResponse();

    _msGraphMock
      .Setup(static x => x.GetGroups(null))
      .ReturnsAsync(groupsCollection);

    var (isSuccessful, _) = await _graphService.CanGetGroups();

    isSuccessful.Should().BeTrue();
    _msGraphMock.Verify(static x => x.GetGroups(null), Times.Once);
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndCannotGetGroups_ItShouldReturnFalse()
  {
    _msGraphMock
      .Setup(static x => x.GetGroups(null))
      .ThrowsAsync(new Exception());

    var (isSuccessful, _) = await _graphService.CanGetGroups();

    isSuccessful.Should().BeFalse();
    _msGraphMock.Verify(static x => x.GetGroups(null), Times.Once);
  }

  [Fact]
  public async Task GetGroupsIterator_WhenCalledAndNoGroupsFound_ItShouldReturnNull()
  {
    var azureGroups = new List<Group>();
    var pageSize = 10;

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _msGraphMock
      .Setup(static x => x.GetGroupsForIterator(It.IsAny<Dictionary<int, string>>(), string.Empty))
      .ReturnsAsync(null as GroupCollectionResponse);

    var result = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    result.Should().BeNull();
    _msGraphMock.Verify(
      static x => x.GetGroupsForIterator(It.IsAny<Dictionary<int, string>>(), string.Empty),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroupsIterator_WhenCalledAndGroupsFound_ItShouldReturnPageIterator()
  {
    var azureGroups = new List<Group>();
    var pageSize = 10;

    var initialGroups = new GroupCollectionResponse
    {
      Value = [
        new Group
        {
          Id = "1",
          Description = "Group 1"
        },
        new Group
        {
          Id = "2",
          Description = "Group 2"
        }
      ]
    };

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _msGraphMock
      .Setup(static x => x.GetGroupsForIterator(It.IsAny<Dictionary<int, string>>(), string.Empty))
      .ReturnsAsync(initialGroups);

    var result = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    result.Should().NotBeNull();
    result.Should().BeOfType<PageIterator<Group, GroupCollectionResponse>>();
    _msGraphMock.Verify(
      static x => x.GetGroupsForIterator(
        It.IsAny<Dictionary<int, string>>(),
        string.Empty
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroupsIterator_WhenCalledAndExceptionIsThrown_ItShouldReturnNull()
  {
    var azureGroups = new List<Group>();
    var pageSize = 10;

    _settingsMock
      .SetupGet(static x => x.Azure)
      .Returns(new AzureSettings());

    _msGraphMock
      .Setup(static x => x.GetGroupsForIterator(It.IsAny<Dictionary<int, string>>(), string.Empty))
      .ThrowsAsync(new Exception());

    var result = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    result.Should().BeNull();
    _msGraphMock.Verify(
      static x => x.GetGroupsForIterator(
        It.IsAny<Dictionary<int, string>>(),
        string.Empty
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanGetUsersAndGroups_ItShouldReturnTrue()
  {
    _msGraphMock
      .Setup(static x => x.GetUsers())
      .ReturnsAsync(new UserCollectionResponse
      {
        Value = [
          new User
          {
            Id = "1",
            UserPrincipalName = "User 1"
          }
        ]
      });

    _msGraphMock
      .Setup(static x => x.GetGroups(null))
      .ReturnsAsync(new GroupCollectionResponse
      {
        Value =
        [
          new Group
          {
            Id = "1",
            Description = "Group 1"
          }
        ]
      });

    var result = await _graphService.IsConnected();

    result.Should().BeTrue();
    _msGraphMock.Verify(static x => x.GetUsers(), Times.Once);
    _msGraphMock.Verify(static x => x.GetGroups(null), Times.Once);
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCannotGetUsers_ItShouldReturnFalse()
  {
    _msGraphMock
      .Setup(static x => x.GetUsers())
      .ThrowsAsync(new Exception());

    var result = await _graphService.IsConnected();

    result.Should().BeFalse();
    _msGraphMock.Verify(static x => x.GetUsers(), Times.Once);
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCannotGetGroups_ItShouldReturnFalse()
  {
    _msGraphMock
      .Setup(static x => x.GetUsers())
      .ReturnsAsync(new UserCollectionResponse
      {
        Value = [
          new User
          {
            Id = "1",
            UserPrincipalName = "User 1"
          }
        ]
      });

    _msGraphMock
      .Setup(static x => x.GetGroups(null))
      .ThrowsAsync(new Exception());

    var result = await _graphService.IsConnected();

    result.Should().BeFalse();
    _msGraphMock.Verify(static x => x.GetUsers(), Times.Once);
    _msGraphMock.Verify(static x => x.GetGroups(null), Times.Once);
  }

  [Fact]
  public async Task GetUserGroups_WhenCalledAndNoGroupsFound_ItShouldReturnEmptyList()
  {
    var user = new User
    {
      Id = "1",
      UserPrincipalName = "User 1"
    };

    _msGraphMock
      .Setup(static x => x.GetUserGroups(It.IsAny<string>()))
      .ReturnsAsync(null as DirectoryObjectCollectionResponse);

    var result = await _graphService.GetUserGroups(user);

    result.Should().NotBeNull();
    result.Should().BeEmpty();

    _msGraphMock.Verify(static x => x.GetUserGroups(It.IsAny<string>()), Times.Once);
  }

  [Fact]
  public async Task GetUserGroups_WhenCalledAndGroupsFound_ItShouldReturnListOfGroups()
  {
    var user = new User
    {
      Id = "1",
      UserPrincipalName = "User 1"
    };

    var groups = new DirectoryObjectCollectionResponse
    {
      Value = [
        new Group
        {
          Id = "1",
          Description = "Group 1"
        },
        new Group
        {
          Id = "2",
          Description = "Group 2"
        }
      ]
    };

    _msGraphMock
      .Setup(static x => x.GetUserGroups(It.IsAny<string>()))
      .ReturnsAsync(groups);

    var result = await _graphService.GetUserGroups(user);

    result.Should().NotBeNull();
    result.Should().NotBeEmpty();
    result.Should().HaveCount(2);

    _msGraphMock.Verify(static x => x.GetUserGroups(It.IsAny<string>()), Times.Once);
  }

  [Fact]
  public async Task GetUserGroups_WhenCalledAndExceptionIsThrown_ItShouldReturnEmptyList()
  {
    var user = new User
    {
      Id = "1",
      UserPrincipalName = "User 1"
    };

    _msGraphMock
      .Setup(static x => x.GetUserGroups(It.IsAny<string>()))
      .ThrowsAsync(new Exception());

    var result = await _graphService.GetUserGroups(user);

    result.Should().NotBeNull();
    result.Should().BeEmpty();

    _msGraphMock.Verify(static x => x.GetUserGroups(It.IsAny<string>()), Times.Once);
  }

  [Fact]
  public async Task GetUsersIterator_WhenCalledAndNoUsersFound_ItShouldReturnNull()
  {
    var azureUsers = new List<User>();
    var pageSize = 10;

    _msGraphMock
      .Setup(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()))
      .ReturnsAsync(null as UserCollectionResponse);

    var result = await _graphService.GetUsersIterator(azureUsers, pageSize);

    result.Should().BeNull();
    _msGraphMock.Verify(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()), Times.Once);
  }

  [Fact]
  public async Task GetUsersIterator_WhenCalledAndUsersFound_ItShouldReturnPageIterator()
  {
    var azureUsers = new List<User>();
    var pageSize = 10;

    var users = new UserCollectionResponse
    {
      Value = [
        new User
        {
          Id = "1",
          UserPrincipalName = "User 1"
        },
        new User
        {
          Id = "2",
          UserPrincipalName = "User 2"
        }
      ]
    };

    _msGraphMock
      .Setup(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()))
      .ReturnsAsync(users);

    var result = await _graphService.GetUsersIterator(azureUsers, pageSize);

    result.Should().NotBeNull();
    result.Should().BeOfType<PageIterator<User, UserCollectionResponse>>();

    _msGraphMock.Verify(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()), Times.Once);
  }

  [Fact]
  public async Task GetUsersIterator_WhenCalledAndExceptionIsThrown_ItShouldReturnNull()
  {
    var azureUsers = new List<User>();
    var pageSize = 10;

    _msGraphMock
      .Setup(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()))
      .Throws(new Exception());

    var result = await _graphService.GetUsersIterator(azureUsers, pageSize);

    result.Should().BeNull();
    _msGraphMock.Verify(static x => x.GetUsersForIterator(It.IsAny<Dictionary<int, string>>()), Times.Once);
  }
}