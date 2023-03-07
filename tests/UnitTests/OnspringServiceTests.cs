namespace OnspringAzureADSyncerTests.UnitTests;

public class OnspringServiceTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<ISettings> _settingsMock;
  private readonly Mock<IOnspringClient> _onspringClientMock;
  private readonly OnspringService _onspringService;

  public OnspringServiceTests()
  {
    _loggerMock = new Mock<ILogger>();
    _settingsMock = new Mock<ISettings>();
    _onspringClientMock = new Mock<IOnspringClient>();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "BaseUrl",
        GroupsAppId = 1,
        UsersAppId = 1,
      }
    );

    _onspringService = new OnspringService(
      _loggerMock.Object,
      _settingsMock.Object,
      _onspringClientMock.Object
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndRequestIsSuccessful_ItShouldReturnTrue()
  {
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
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.CanGetGroups();

    result.Should().BeTrue();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Once
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    var apiResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndHttpRequestExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Throws(new HttpRequestException());

    var result = await _onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndTaskCanceledExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Throws(new TaskCanceledException());

    var result = await _onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetGroups_WhenCalledAndExceptionIsThrown_ItShouldReturnFalse()
  {
    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.CanGetGroups();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Once
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndRequestIsSuccessful_ItShouldReturnTrue()
  {
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
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.CanGetUsers();

    result.Should().BeTrue();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Once
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    var apiResponse = new ApiResponse<App>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndHttpRequestExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Throws(new HttpRequestException());

    var result = await _onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CanGetUsers_WhenCalledAndTaskCanceledExceptionIsThrown_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Throws(new TaskCanceledException());

    var result = await _onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(3)
    );
  }


  [Fact]
  public async Task CanGetUsers_WhenCalledAndExceptionIsThrown_ItShouldReturnFalse()
  {
    _onspringClientMock
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.CanGetUsers();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Once
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanGetUsersAndGroups_ItShouldReturnTrue()
  {
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
    .Setup(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.IsConnected();

    result.Should().BeTrue();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(2)
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanNotGetUsers_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
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
    .SetupSequence(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Returns(failureResponse)
    .Returns(failureResponse)
    .Returns(failureResponse);

    var result = await _onspringService.IsConnected();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task IsConnected_WhenCalledAndCanNotGetGroups_ItShouldReturnFalseAfterRetryingThreeTimes()
  {
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
    .SetupSequence(
      m => m.GetAppAsync(It.IsAny<int>()).Result
    )
    .Returns(successResponse)
    .Returns(failureResponse)
    .Returns(failureResponse)
    .Returns(failureResponse);

    var result = await _onspringService.IsConnected();

    result.Should().BeFalse();
    _onspringClientMock.Verify(
      m => m.GetAppAsync(It.IsAny<int>()),
      Times.Exactly(4)
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndOnePageOfFieldsAreFound_ItShouldReturnAListOfFields()
  {
    var listOfFields = new List<Field>
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
    };

    var pageOfFields = new GetPagedFieldsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = listOfFields,
    };

    var apiResponse = new ApiResponse<GetPagedFieldsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pageOfFields
    };

    _onspringClientMock
    .Setup(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.GetGroupFields();

    result.Should().NotBeNull();
    result.Should().BeOfType<List<Field>>();
    result.Should().HaveCount(1);
    result.Should().BeEquivalentTo(listOfFields);
    _onspringClientMock.Verify(
      m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndMultiplePagesOfFieldsAreFound_ItShouldReturnAListOfFields()
  {
    var listOfFields = new List<Field>
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
    };

    var pageOneOfFields = new GetPagedFieldsResponse
    {
      TotalPages = 2,
      TotalRecords = 2,
      PageNumber = 1,
      Items = listOfFields,
    };

    var pageTwoOfFields = new GetPagedFieldsResponse
    {
      TotalPages = 2,
      TotalRecords = 2,
      PageNumber = 2,
      Items = listOfFields,
    };

    var pageOneResponse = new ApiResponse<GetPagedFieldsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pageOneOfFields
    };

    var pageTwoResponse = new ApiResponse<GetPagedFieldsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pageTwoOfFields
    };

    _onspringClientMock
    .SetupSequence(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(pageOneResponse)
    .Returns(pageTwoResponse);

    var result = await _onspringService.GetGroupFields();

    result.Should().NotBeNull();
    result.Should().BeOfType<List<Field>>();
    result.Should().HaveCount(2);
    result.Should().BeEquivalentTo(new List<Field> { listOfFields[0], listOfFields[0] });
    _onspringClientMock.Verify(
      m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(2)
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndFieldsAreNotFound_ItShouldReturnAnEmptyList()
  {
    var listOfFields = new List<Field>();

    var pageOfFields = new GetPagedFieldsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = listOfFields,
    };

    var apiResponse = new ApiResponse<GetPagedFieldsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pageOfFields
    };

    _onspringClientMock
    .Setup(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.GetGroupFields();

    result.Should().NotBeNull();
    result.Should().BeOfType<List<Field>>();
    result.Should().BeEmpty();
    _onspringClientMock.Verify(
      m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndOnePageOfFieldsIsFoundAndExceptionIsThrown_ItShouldReturnAnEmptyList()
  {
    _onspringClientMock
    .Setup(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.GetGroupFields();

    result.Should().NotBeNull();
    result.Should().BeOfType<List<Field>>();
    result.Should().BeEmpty();
    _onspringClientMock.Verify(
      m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndMultiplePagesOfFieldsAreFoundAndAnExceptionIsThrown_ItShouldReturnAListOfFieldsFromSuccessfulPages()
  {
    var listOfFields = new List<Field>
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
    };

    var pageOneOfFields = new GetPagedFieldsResponse
    {
      TotalPages = 2,
      TotalRecords = 2,
      PageNumber = 1,
      Items = listOfFields,
    };

    var pageOneResponse = new ApiResponse<GetPagedFieldsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pageOneOfFields
    };

    _onspringClientMock
    .SetupSequence(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(pageOneResponse)
    .Throws(new Exception());

    var result = await _onspringService.GetGroupFields();

    result.Should().NotBeNull();
    result.Should().BeOfType<List<Field>>();
    result.Should().HaveCount(1);
    result.Should().BeEquivalentTo(new List<Field> { listOfFields[0], });
    _onspringClientMock.Verify(
      m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(2)
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndAnHttpRequestExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnAnEmptyListAfterRetryingThreeTimes()
  {
    _onspringClientMock
    .SetupSequence(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Throws(new HttpRequestException())
    .Throws(new TaskCanceledException())
    .Throws(new HttpRequestException());

    var result = await _onspringService.GetGroupFields();

    result.Should().NotBeNull();
    result.Should().BeOfType<List<Field>>();
    result.Should().BeEmpty();
    _onspringClientMock.Verify(
      m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task GetGroupFields_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnAnEmptyListAfterRetryingThreeTimes()
  {
    var apiResponse = new ApiResponse<GetPagedFieldsResponse>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.GetGroupFields();

    result.Should().NotBeNull();
    result.Should().BeOfType<List<Field>>();
    result.Should().BeEmpty();
    _onspringClientMock.Verify(
      m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(3)
    );
  }
}