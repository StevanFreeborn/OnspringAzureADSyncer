namespace OnspringAzureADSyncerTests.UnitTests;

public class OnspringServiceTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<ISettings> _settingsMock;
  private readonly Mock<IOnspringClient> _onspringClientMock;

  public OnspringServiceTests()
  {
    _loggerMock = new Mock<ILogger>();
    _settingsMock = new Mock<ISettings>();
    _onspringClientMock = new Mock<IOnspringClient>();
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndRequestIsSuccessful_ItShouldReturnTrue()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    var apiResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.OK,
      Value = new App
      {
        Id = 1,
        Name = "Name",
        Href = "Href",
      },
    };

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Returns(apiResponse);

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetGroups();

    result.Should().BeTrue();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Once
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    var apiResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Returns(apiResponse);

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndHttpRequestExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Throws(new HttpRequestException());

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndTaskCanceledExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Throws(new TaskCanceledException());

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndExceptionIsThrown_ItShouldReturnFalse()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Throws(new Exception());

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Once
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndRequestIsSuccessful_ItShouldReturnTrue()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    var apiResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.OK,
      Value = new App
      {
        Id = 1,
        Name = "Name",
        Href = "Href",
      },
    };

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Returns(apiResponse);

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetUsers();

    result.Should().BeTrue();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Once
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    var apiResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Returns(apiResponse);

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndHttpRequestExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Throws(new HttpRequestException());

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndTaskCanceledExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Throws(new TaskCanceledException());

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(3)
    );
  }


  [Fact]
  public async Task CanGetUsers_WhenCalledAndExceptionIsThrown_ItShouldReturnFalse()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Throws(new Exception());

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Once
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanGetUsersAndGroups_ItShouldReturnTrue()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    var apiResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.OK,
      Value = new App
      {
        Id = 1,
        Name = "Name",
        Href = "Href",
      },
    };

    _onspringClientMock
    .Setup(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Returns(apiResponse);

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.IsConnected();

    result.Should().BeTrue();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(2)
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanNotGetUsers_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    var successResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.OK,
      Value = new App
      {
        Id = 1,
        Name = "Name",
        Href = "Href",
      },
    };

    var failureResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .SetupSequence(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Returns(failureResponse)
    .Returns(failureResponse)
    .Returns(failureResponse);

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.IsConnected();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(3)
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanNotGetGroups_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _settingsMock
    .SetupGet(x => x.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    var successResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.OK,
      Value = new App
      {
        Id = 1,
        Name = "Name",
        Href = "Href",
      },
    };

    var failureResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .SetupSequence(m => m.GetAppAsync(It.IsAny<int>()).Result)
    .Returns(successResponse)
    .Returns(failureResponse)
    .Returns(failureResponse)
    .Returns(failureResponse);

    var onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );

    var result = await onspringService.IsConnected();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()), Times.Exactly(4)
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndOnePageOfFieldsAreFound_ItShouldReturnAListOfFields()
  {

  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndMultiplePagesOfFieldsAreFound_ItShouldReturnAListOfFields()
  {

  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndFieldsAreNotFound_ItShouldReturnAnEmptyList()
  {

  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndExceptionIsThrown_ItShouldReturnAnEmptyList()
  {

  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndASinglePageOfFieldsIsFoundAndAnExceptionIsThrown_ItShouldReturnAnEmptyList()
  {

  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndMultiplePagesOfFieldsAreFoundAndAnExceptionIsThrown_ItShouldReturnAListOfFieldsFromSuccessfulPages()
  {

  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndMultiplePagesOfFieldsAreFoundAndAnExceptionIsThrown_ItShouldReturnAnEmptyListIfNoSuccessfulPages()
  {

  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndAnHttpExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnAnEmptyListAfterRetryingThreeTimes()
  {

  }
}