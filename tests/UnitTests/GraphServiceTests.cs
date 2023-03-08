using static Microsoft.Graph.Users.UsersRequestBuilder;

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
    .SetupGet(
      x => x.GraphServiceClient
    )
    .Returns(
      new GraphServiceClient(tokenCredentialMock.Object, null, null)
    );

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
    .Setup(
      x => x.GetUsers().Result
    )
    .Returns(usersCollection);

    var result = await _graphService.CanGetUsers();

    result.Should().BeTrue();
    _msGraphMock.Verify(
      x => x.GetUsers(),
      Times.Once
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndCannotGetUsers_ItShouldReturnFalse()
  {
    _msGraphMock
    .Setup(
      x => x.GetUsers().Result
    )
    .Throws(new Exception());

    var result = await _graphService.CanGetUsers();

    result.Should().BeFalse();
    _msGraphMock.Verify(
      x => x.GetUsers(),
      Times.Once
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndCanGetGroups_ItShouldReturnTrue()
  {
    var groupsCollection = new GroupCollectionResponse();

    _msGraphMock
    .Setup(
      x => x.GetGroups().Result
    )
    .Returns(groupsCollection);

    var result = await _graphService.CanGetGroups();

    result.Should().BeTrue();
    _msGraphMock.Verify(
      x => x.GetGroups(),
      Times.Once
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndCannotGetGroups_ItShouldReturnFalse()
  {
    _msGraphMock
    .Setup(
      x => x.GetGroups().Result
    )
    .Throws(new Exception());

    var result = await _graphService.CanGetGroups();

    result.Should().BeFalse();
    _msGraphMock.Verify(
      x => x.GetGroups(),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroupsIterator_WhenCalledAndNoGroupsFound_ItShouldReturnNull()
  {
    var azureGroups = new List<Group>();
    var pageSize = 10;

    _msGraphMock
    .Setup(
      x => x.GetGroupsForIterator(It.IsAny<Dictionary<string, int>>()).Result
    )
    .Returns<GroupCollectionResponse>(null);

    var result = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    result.Should().BeNull();
    _msGraphMock.Verify(
      x => x.GetGroupsForIterator(It.IsAny<Dictionary<string, int>>()),
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
      Value = new List<Group>
      {
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
      }
    };

    _msGraphMock
    .Setup(
      x => x.GetGroupsForIterator(It.IsAny<Dictionary<string, int>>()).Result
    )
    .Returns(initialGroups);

    var result = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    result.Should().NotBeNull();
    result.Should().BeOfType<PageIterator<Group, GroupCollectionResponse>>();
    _msGraphMock.Verify(
      x => x.GetGroupsForIterator(It.IsAny<Dictionary<string, int>>()),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroupsIterator_WhenCalledAndExceptionIsThrown_ItShouldReturnNull()
  {
    var azureGroups = new List<Group>();
    var pageSize = 10;

    _msGraphMock
    .Setup(
      x => x.GetGroupsForIterator(It.IsAny<Dictionary<string, int>>()).Result
    )
    .Throws(new Exception());

    var result = await _graphService.GetGroupsIterator(azureGroups, pageSize);

    result.Should().BeNull();
    _msGraphMock.Verify(
      x => x.GetGroupsForIterator(It.IsAny<Dictionary<string, int>>()),
      Times.Once
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanGetUsersAndGroups_ItShouldReturnTrue()
  {
    _msGraphMock
    .Setup(
      x => x.GetUsers().Result
    )
    .Returns(
      new UserCollectionResponse
      {
        Value = new List<User>
        {
          new User
          {
            Id = "1",
            UserPrincipalName = "User 1"
          }
        }
      }
    );

    _msGraphMock
    .Setup(
      x => x.GetGroups().Result
    )
    .Returns(
      new GroupCollectionResponse
      {
        Value = new List<Group>
        {
          new Group
          {
            Id = "1",
            Description = "Group 1"
          }
        }
      }
    );

    var result = await _graphService.IsConnected();

    result.Should().BeTrue();
    _msGraphMock.Verify(
      x => x.GetUsers(),
      Times.Once
    );
    _msGraphMock.Verify(
      x => x.GetGroups(),
      Times.Once
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCannotGetUsers_ItShouldReturnFalse()
  {
    _msGraphMock
    .Setup(
      x => x.GetUsers().Result
    )
    .Throws(new Exception());

    var result = await _graphService.IsConnected();

    result.Should().BeFalse();
    _msGraphMock.Verify(
      x => x.GetUsers(),
      Times.Once
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCannotGetGroups_ItShouldReturnFalse()
  {
    _msGraphMock
    .Setup(
      x => x.GetUsers().Result
    )
    .Returns(
      new UserCollectionResponse
      {
        Value = new List<User>
        {
          new User
          {
            Id = "1",
            UserPrincipalName = "User 1"
          }
        }
      }
    );

    _msGraphMock
    .Setup(
      x => x.GetGroups().Result
    )
    .Throws(new Exception());

    var result = await _graphService.IsConnected();

    result.Should().BeFalse();
    _msGraphMock.Verify(
      x => x.GetUsers(),
      Times.Once
    );
    _msGraphMock.Verify(
      x => x.GetGroups(),
      Times.Once
    );
  }
}