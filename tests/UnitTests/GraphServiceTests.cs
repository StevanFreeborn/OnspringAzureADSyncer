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
    .Setup(x => x.GetUsers())
    .ReturnsAsync(usersCollection);

    var result = await _graphService.CanGetUsers();

    result.Should().BeTrue();
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndCannotGetUsers_ItSHouldReturnFalse()
  {

  }
}