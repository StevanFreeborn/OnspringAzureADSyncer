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

    _settingsMock
    .SetupGet(m => m.GroupsFieldMappings)
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "id" },
        { 2, "description" },
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

  [Fact]
  public async Task GetGroup_WhenCalledAndGroupIsFound_ItShouldReturnAGroup()
  {
    var groups = new List<ResultRecord>
    {
      new ResultRecord()
    };

    var pagedResponse = new GetPagedRecordsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = groups,
    };

    var response = new ApiResponse<GetPagedRecordsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pagedResponse,
    };

    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.GetGroup(It.IsAny<string>());

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.Should().BeEquivalentTo(groups[0]);
    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroup_WhenCalledAndGroupIsNotFound_ItShouldReturnNull()
  {
    var groups = new List<ResultRecord>();

    var pagedResponse = new GetPagedRecordsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = groups,
    };

    var response = new ApiResponse<GetPagedRecordsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pagedResponse,
    };

    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.GetGroup(It.IsAny<string>());

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroup_WhenCalledAndAnExceptionIsThrown_ItShouldReturnNull()
  {
    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.GetGroup(It.IsAny<string>());

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetGroup_WhenCalledAndAnHttpExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    _onspringClientMock
    .SetupSequence(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Throws(new HttpRequestException())
    .Throws(new TaskCanceledException())
    .Throws(new HttpRequestException());

    var result = await _onspringService.GetGroup(It.IsAny<string>());

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task GetGroup_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var response = new ApiResponse<GetPagedRecordsResponse>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.GetGroup(It.IsAny<string>());

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CreateGroup_WhenCalledAndCanBuildNewGroupWithFieldDataAndRequestIsSuccessful_ItShouldReturnSaveRecordResponse()
  {
    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 123,
    };

    var response = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = saveRecordResponse,
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.CreateGroup(azureGroup);

    result.Should().NotBeNull();
    result.Should().BeOfType<SaveRecordResponse>();
    result.Should().BeEquivalentTo(saveRecordResponse);
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task CreateGroup_WhenCalledAndCannotBuildNewGroupWithFieldData_ItShouldReturnNull()
  {
    var azureGroup = new Group();

    var result = await _onspringService.CreateGroup(azureGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Never
    );
  }

  [Fact]
  public async Task CreateGroup_WhenCalledAndAnExceptionIsThrown_ItShouldReturnNull()
  {
    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.CreateGroup(azureGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task CreateGroup_WhenCalledAndAnHttpExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    _onspringClientMock
    .SetupSequence(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new HttpRequestException())
    .Throws(new TaskCanceledException())
    .Throws(new HttpRequestException());

    var result = await _onspringService.CreateGroup(azureGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CreateGroup_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    var response = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.CreateGroup(azureGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task UpdateGroup_WhenCalledAndCanBuildAnUpdatedRecordAndRequestIsSuccessful_ItShouldReturnASaveRecordResponse()
  {
    var onspringGroup = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, ""),
        new StringFieldValue(2, ""),
      },
    };

    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 123,
    };

    var response = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = saveRecordResponse,
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.UpdateGroup(azureGroup, onspringGroup);

    result.Should().NotBeNull();
    result.Should().BeOfType<SaveRecordResponse>();
    result.Should().BeEquivalentTo(saveRecordResponse);
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task UpdateGroup_WhenCalledAndCannotBuildAnUpdatedRecord_ItShouldReturnNull()
  {
    var onspringGroup = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "464a535b-bbc8-4c18-bb12-9f1596464d43"),
        new StringFieldValue(2, "Test Group Description"),
      },
    };

    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 123,
    };

    var response = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = saveRecordResponse,
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.UpdateGroup(azureGroup, onspringGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Never
    );
  }

  [Fact]
  public async Task UpdateGroup_WhenCalledAndAnExceptionIsThrown_ItShouldReturnNull()
  {
    var onspringGroup = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, ""),
        new StringFieldValue(2, ""),
      },
    };

    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.UpdateGroup(azureGroup, onspringGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task UpdateGroup_WhenCalledAndAnHttpExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var onspringGroup = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, ""),
        new StringFieldValue(2, ""),
      },
    };

    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    _onspringClientMock
    .SetupSequence(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new HttpRequestException())
    .Throws(new TaskCanceledException())
    .Throws(new TaskCanceledException());

    var result = await _onspringService.UpdateGroup(azureGroup, onspringGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task UpdateGroup_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var onspringGroup = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, ""),
        new StringFieldValue(2, ""),
      },
    };

    var azureGroup = new Group
    {
      Id = "464a535b-bbc8-4c18-bb12-9f1596464d43",
      Description = "Test Group Description",
    };

    var apiResponse = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.UpdateGroup(azureGroup, onspringGroup);

    result.Should().BeNull();
    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task UpdateUserWhenCalledAndCanBuildAnUpdatedRecordAndRequestIsSuccessful_ItShouldReturnASaveRecordResponse()
  {
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "User1"),
        new StringFieldValue(2, "First Name"),
      },
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "Updated First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 123,
    };

    var response = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = saveRecordResponse,
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.UpdateUser(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<SaveRecordResponse>();
    result.Should().BeEquivalentTo(saveRecordResponse);

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task UpdateUser_WhenCalledAndCannotBuildAnUpdatedRecord_ItShouldReturnNull()
  {
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "User1"),
        new StringFieldValue(2, "First Name"),
      },
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>();

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    var result = await _onspringService.UpdateUser(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Never
    );
  }

  [Fact]
  public async Task UpdateUser_WhenCalledAndAnExceptionIsThrown_ItShouldReturnNull()
  {
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "User1"),
        new StringFieldValue(2, "First Name"),
      },
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "Updated First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.UpdateUser(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task UpdateUser_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "User1"),
        new StringFieldValue(2, "First Name"),
      },
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "Updated First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    var apiResponse = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.UpdateUser(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task UpdateUser_WhenCalledAndHttpRequestExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "User1"),
        new StringFieldValue(2, "First Name"),
      },
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "Updated First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    _onspringClientMock
    .SetupSequence(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new HttpRequestException())
    .Throws(new TaskCanceledException())
    .Throws(new HttpRequestException());

    var result = await _onspringService.UpdateUser(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }


  [Fact]
  public async Task CreateUser_WhenCalledAndCanBuildNewUserWithFieldDataAndRequestIsSuccessful_ItShouldReturnSaveRecordResponse()
  {
    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    var saveRecordResponse = new SaveRecordResponse
    {
      Id = 1,
      Warnings = new List<string>(),
    };

    var apiResponse = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = saveRecordResponse,
    };

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(
      apiResponse
    );

    var result = await _onspringService.CreateUser(
      azureUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<SaveRecordResponse>();
    result.Should().BeEquivalentTo(saveRecordResponse);

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task CreateUser_WhenCalledAndCannotBuildNewUserWithFieldData_ItShouldReturnNull()
  {
    var azureUser = new User();

    var usersGroupMappings = new Dictionary<string, int>();

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>()
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    var result = await _onspringService.CreateUser(
      azureUser,
      usersGroupMappings
    );

    result.Should().BeNull();
  }

  [Fact]
  public async Task CreateUser_WhenCalledAndAnExceptionIsThrown_ItShouldReturnNull()
  {
    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.CreateUser(
      azureUser,
      usersGroupMappings
    );

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task CreateUser_WhenCalledAndAnHttpExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    _onspringClientMock
    .SetupSequence(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Throws(new HttpRequestException())
    .Throws(new TaskCanceledException())
    .Throws(new HttpRequestException());

    var result = await _onspringService.CreateUser(
      azureUser,
      usersGroupMappings
    );

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task CreateUser_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    var usersGroupMappings = new Dictionary<string, int>
    {
      { "Group Id 1", 1 },
    };

    var apiResponse = new ApiResponse<SaveRecordResponse>
    {
      StatusCode = HttpStatusCode.InternalServerError,
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Azure
    )
    .Returns(
      new AzureSettings()
    );

    _onspringClientMock
    .Setup(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ).Result
    )
    .Returns(apiResponse);

    var result = await _onspringService.CreateUser(
      azureUser,
      usersGroupMappings
    );

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.SaveRecordAsync(
        It.IsAny<ResultRecord>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task GetUser_WhenCalledAndUserIsFound_ItShouldReturnAUser()
  {
    var users = new List<ResultRecord>
    {
      new ResultRecord()
    };

    var pagedResponse = new GetPagedRecordsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = users,
    };

    var response = new ApiResponse<GetPagedRecordsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pagedResponse,
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Onspring
    )
    .Returns(
      new OnspringSettings
      {
        UsersUsernameFieldId = 1,
      }
    );

    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.GetUser(azureUser);

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.Should().BeEquivalentTo(users[0]);

    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetUser_WhenCalledAndUserIsNotFound_ItShouldReturnNull()
  {
    var users = new List<ResultRecord>();

    var pagedResponse = new GetPagedRecordsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = users,
    };

    var response = new ApiResponse<GetPagedRecordsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pagedResponse,
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Onspring
    )
    .Returns(
      new OnspringSettings
      {
        UsersUsernameFieldId = 1,
      }
    );

    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.GetUser(azureUser);

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetUser_WhenCalledAndPropertyMappedToUsernameIsNotValidAndUserIsNotFound_ItShouldReturnNull()
  {
    var users = new List<ResultRecord>();

    var pagedResponse = new GetPagedRecordsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = users,
    };

    var response = new ApiResponse<GetPagedRecordsResponse>
    {
      StatusCode = HttpStatusCode.OK,
      Value = pagedResponse,
    };

    var azureUser = new User();

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "invalidProperty" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Onspring
    )
    .Returns(
      new OnspringSettings
      {
        UsersUsernameFieldId = 1,
      }
    );

    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(response);

    var result = await _onspringService.GetUser(azureUser);

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetUser_WhenCalledAndAnExceptionIsThrown_ItShouldReturnNull()
  {
    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Onspring
    )
    .Returns(
      new OnspringSettings
      {
        UsersUsernameFieldId = 1,
      }
    );

    _onspringClientMock
    .Setup(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.GetUser(azureUser);

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Once
    );
  }

  [Fact]
  public async Task GetUser_WhenCalledAndAnHttpExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Onspring
    )
    .Returns(
      new OnspringSettings
      {
        UsersUsernameFieldId = 1,
      }
    );

    _onspringClientMock
    .SetupSequence(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Throws(new HttpRequestException())
    .Throws(new TaskCanceledException())
    .Throws(new HttpRequestException());

    var result = await _onspringService.GetUser(azureUser);

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task GetUser_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnNullAfterRetryingThreeTimes()
  {
    var users = new List<ResultRecord>();

    var pagedResponse = new GetPagedRecordsResponse
    {
      TotalPages = 1,
      TotalRecords = 1,
      PageNumber = 1,
      Items = users,
    };

    var response = new ApiResponse<GetPagedRecordsResponse>
    {
      StatusCode = HttpStatusCode.BadRequest,
      Value = pagedResponse,
    };

    var azureUser = new User
    {
      UserPrincipalName = "User1",
      GivenName = "First Name",
    };

    _settingsMock
    .SetupGet(
      m => m.UsersFieldMappings
    )
    .Returns(
      new Dictionary<int, string>
      {
        { 1, "userPrincipalName" },
        { 2, "givenName" },
      }
    );

    _settingsMock
    .SetupGet(
      m => m.Onspring
    )
    .Returns(
      new OnspringSettings
      {
        UsersUsernameFieldId = 1,
      }
    );

    _onspringClientMock
    .SetupSequence(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Returns(response)
    .Returns(response)
    .Returns(response);

    var result = await _onspringService.GetUser(azureUser);

    result.Should().BeNull();

    _onspringClientMock.Verify(
      m => m.QueryRecordsAsync(
        It.IsAny<QueryRecordsRequest>(),
        It.IsAny<PagingRequest>()
      ),
      Times.Exactly(3)
    );
  }

  [Fact]
  public async Task GetUserFields_WhenCalledAndOnePageOfFieldsAreFound_ItShouldReturnAListOfFields()
  {
    var listOfFields = new List<Field>
    {
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

    var result = await _onspringService.GetUserFields();

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
  public async Task GetUserFields_WhenCalledAndMultiplePagesOfFieldsAreFound_ItShouldReturnAListOfFields()
  {
    var listOfFields = new List<Field>
    {
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

    var result = await _onspringService.GetUserFields();

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
  public async Task GetUserFields_WhenCalledAndFieldsAreNotFound_ItShouldReturnAnEmptyList()
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

    var result = await _onspringService.GetUserFields();

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
  public async Task GetUserFields_WhenCalledAndOnePageOfFieldsIsFoundAndExceptionIsThrown_ItShouldReturnAnEmptyList()
  {
    _onspringClientMock
    .Setup(m => m.GetFieldsForAppAsync(
        It.IsAny<int>(),
        It.IsAny<PagingRequest>()
      ).Result
    )
    .Throws(new Exception());

    var result = await _onspringService.GetUserFields();

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
  public async Task GetUserFields_WhenCalledAndMultiplePagesOfFieldsAreFoundAndAnExceptionIsThrown_ItShouldReturnAListOfFieldsFromSuccessfulPages()
  {
    var listOfFields = new List<Field>
    {
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

    var result = await _onspringService.GetUserFields();

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
  public async Task GetUserFields_WhenCalledAndAnHttpRequestExceptionOrTaskCanceledExceptionIsThrown_ItShouldReturnAnEmptyListAfterRetryingThreeTimes()
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

    var result = await _onspringService.GetUserFields();

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
  public async Task GetUserFields_WhenCalledAndRequestIsUnsuccessful_ItShouldReturnAnEmptyListAfterRetryingThreeTimes()
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

    var result = await _onspringService.GetUserFields();

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
  public void BuildUpdatedOnspringUserRecord_WhenCalledAndUsersStatusIsNotNull_ItShouldAddUsersStatusToFieldData()
  {
    var usersGroupMappings = new Dictionary<string, int>();

    var azureUser = new User
    {
      Id = "1",
      AccountEnabled = true,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "Active"),
      },
    };

    _settingsMock
    .Setup(m => m.UsersFieldMappings)
    .Returns(new Dictionary<int, string>());

    _settingsMock
    .Setup(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = Guid.NewGuid(),
        UserInactiveStatusListValue = Guid.NewGuid(),
      }
    );

    _settingsMock
    .Setup(m => m.Azure)
    .Returns(
      new AzureSettings()
    );

    var result = _onspringService.BuildUpdatedOnspringUserRecord(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.FieldData.Should().HaveCount(1);
    result.FieldData[0].FieldId.Should().Be(1);
  }

  [Fact]
  public void BuildUpdatedOnspringUserRecord_WhenCalledAndUsersStatusIsNull_ItShouldNotAddUsersStatusToFieldData()
  {
    var usersGroupMappings = new Dictionary<string, int>();

    var azureUser = new User
    {
      Id = "1",
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>(),
    };

    _settingsMock
    .Setup(m => m.UsersFieldMappings)
    .Returns(new Dictionary<int, string>());

    _settingsMock
    .Setup(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = Guid.NewGuid(),
        UserInactiveStatusListValue = Guid.NewGuid(),
      }
    );

    _settingsMock
    .Setup(m => m.Azure)
    .Returns(
      new AzureSettings()
    );

    var result = _onspringService.BuildUpdatedOnspringUserRecord(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.FieldData.Should().BeEmpty();
  }

  [Fact]
  public void BuildUpdatedOnspringUserRecord_WhenCalledAndExistingUserHasNoGroups_ItShouldAddUpdatedGroupsFieldToFieldData()
  {
    var usersGroupMappings = new Dictionary<string, int>
    {
      { "1", 1 },
    };

    var azureUser = new User
    {
      Id = "1",
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>(),
    };

    _settingsMock
    .Setup(m => m.UsersFieldMappings)
    .Returns(new Dictionary<int, string>());

    _settingsMock
    .Setup(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UsersGroupsFieldId = 2,
        UserActiveStatusListValue = Guid.NewGuid(),
        UserInactiveStatusListValue = Guid.NewGuid(),
      }
    );
  }

  [Fact]
  public void BuildUpdatedOnspringUserRecord_WhenCalledAndExistingUserNeedsGroupsUpdated_ItShouldAddUpdatedGroupsFieldToFieldData()
  {
    var usersGroupMappings = new Dictionary<string, int>
    {
      { "1", 1 },
    };

    var azureUser = new User
    {
      Id = "1",
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new IntegerListFieldValue(
          2,
          new List<int>
          {
            2,
          }
        ),
      },
    };

    _settingsMock
    .Setup(m => m.UsersFieldMappings)
    .Returns(new Dictionary<int, string>());

    _settingsMock
    .Setup(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UsersGroupsFieldId = 2,
        UserActiveStatusListValue = Guid.NewGuid(),
        UserInactiveStatusListValue = Guid.NewGuid(),
      }
    );

    _settingsMock
    .Setup(m => m.Azure)
    .Returns(
      new AzureSettings()
    );

    var result = _onspringService.BuildUpdatedOnspringUserRecord(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.FieldData.Should().HaveCount(1);
    result.FieldData[0].FieldId.Should().Be(2);
  }

  [Fact]
  public void BuildUpdatedOnspringUserRecord_WhenCalledAndUsersGroupDoesNotNeedUpdated_ItShouldNotAddUpdatedGroupsFieldToFieldData()
  {
    var usersGroupMappings = new Dictionary<string, int>
    {
      { "1", 1 },
    };

    var azureUser = new User
    {
      Id = "1",
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new IntegerListFieldValue(
          2,
          new List<int>
          {
            1,
          }
        ),
      },
    };

    _settingsMock
    .Setup(m => m.UsersFieldMappings)
    .Returns(new Dictionary<int, string>());

    _settingsMock
    .Setup(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UsersGroupsFieldId = 2,
        UserActiveStatusListValue = Guid.NewGuid(),
        UserInactiveStatusListValue = Guid.NewGuid(),
      }
    );

    _settingsMock
    .Setup(m => m.Azure)
    .Returns(
      new AzureSettings()
    );

    var result = _onspringService.BuildUpdatedOnspringUserRecord(
      azureUser,
      onspringUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.FieldData.Should().BeEmpty();
  }

  [Fact]
  public void BuildNewOnspringUserRecord_WhenCalledAndUsersStatusIsNotNull_ItShouldAddUsersStatusToFieldData()
  {
    var usersGroupMappings = new Dictionary<string, int>();

    var azureUser = new User
    {
      AccountEnabled = true,
    };

    _settingsMock
    .Setup(m => m.UsersFieldMappings)
    .Returns(new Dictionary<int, string>());

    _settingsMock
    .Setup(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = Guid.NewGuid(),
        UserInactiveStatusListValue = Guid.NewGuid(),
      }
    );

    _settingsMock
    .Setup(m => m.Azure)
    .Returns(
      new AzureSettings()
    );

    var result = _onspringService.BuildNewOnspringUserRecord(
      azureUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.FieldData.Should().HaveCount(1);
    result.FieldData[0].FieldId.Should().Be(1);
  }

  [Fact]
  public void BuildNewOnspringUserRecord_WhenCalledAndUsersStatusIsNull_ItShouldNotAddUsersStatusToFieldData()
  {
    var usersGroupMappings = new Dictionary<string, int>();

    var azureUser = new User();

    _settingsMock
    .Setup(m => m.UsersFieldMappings)
    .Returns(new Dictionary<int, string>());

    _settingsMock
    .Setup(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = Guid.NewGuid(),
        UserInactiveStatusListValue = Guid.NewGuid(),
      }
    );

    _settingsMock
    .Setup(m => m.Azure)
    .Returns(
      new AzureSettings()
    );

    var result = _onspringService.BuildNewOnspringUserRecord(
      azureUser,
      usersGroupMappings
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<ResultRecord>();
    result.FieldData.Should().BeEmpty();
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndAzureUserAccountEnabledPropertyIsNull_ItShouldReturnNull()
  {
    var azureUser = new User();
    var userGroupIds = new List<string>();

    var result = _onspringService.GetUsersStatus(
      azureUser,
      null,
      userGroupIds
    );

    result.Should().BeNull();
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNullAndAzureUserIsEnabledAndAMemberOfAnOnspringActiveGroup_ItShouldReturnActiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      null,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(activeListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNullAndAzureUserIsNotEnabled_ItShouldReturnInactiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = false,
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      null,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(inactiveListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNullAndAzureUserIsNotPartOfAnOnspringActiveGroup_ItShouldReturnInactiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var userGroupIds = new List<string>();

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      null,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(inactiveListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullAndAzureUserIsEnabledAndAMemberOfAnOnspringActiveGroup_ItShouldReturnActiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "Inactive")
      },
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(activeListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullAndAzureUserIsNotEnabled_ItShouldReturnInactiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = false,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "Active")
      },
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(inactiveListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullAndAzureUserIsNotPartOfAnOnspringActiveGroup_ItShouldReturnInactiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "Active")
      },
    };

    var userGroupIds = new List<string>();

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(inactiveListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullAndExistingStatusIsInactiveAndAzureUserIsNotEnabled_ItShouldReturnNull()
  {
    var azureUser = new User
    {
      AccountEnabled = false,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "Inactive")
      },
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
      .Returns(
        new OnspringSettings
        {
          UsersStatusFieldId = 1,
          UserActiveStatusListValue = activeListValueId,
          UserInactiveStatusListValue = inactiveListValueId,
        }
      );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().BeNull();
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullAndExistingStatusIsInactiveAndAzureUserIsNotPartOfAnOnspringActiveGroup_ItShouldReturnNull()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "Inactive")
      },
    };

    var userGroupIds = new List<string>();

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().BeNull();
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullAndExistingStatusIsActiveAndAzureUserIsEnabledAndAzureUserIsNotPartOfAnOnspringActiveGroup_ItShouldReturnNull()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>
      {
        new StringFieldValue(1, "Active")
      },
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().BeNull();
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullButExistingStatusIsNullAndAzureUserIsEnabledAndAMemberOfAnOnspringActiveGroup_ItShouldReturnActiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>(),
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(activeListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullButExistingStatusIsNullAndAzureUserIsNotEnabled_ItShouldReturnInactiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = false,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>(),
    };

    var userGroupIds = new List<string>
    {
      "1",
    };

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(inactiveListValueId.ToString());
  }

  [Fact]
  public void GetUsersStatus_WhenCalledAndOnspringUserIsNotNullButExistingStatusIsNullAndAzureUserIsNotPartOfAnOnspringActiveGroup_ItShouldReturnInactiveStatusFieldValue()
  {
    var azureUser = new User
    {
      AccountEnabled = true,
    };

    var onspringUser = new ResultRecord
    {
      AppId = 1,
      RecordId = 1,
      FieldData = new List<RecordFieldValue>(),
    };

    var userGroupIds = new List<string>();

    var activeListValueId = Guid.NewGuid();
    var inactiveListValueId = Guid.NewGuid();

    _settingsMock
    .SetupGet(m => m.Onspring)
    .Returns(
      new OnspringSettings
      {
        UsersStatusFieldId = 1,
        UserActiveStatusListValue = activeListValueId,
        UserInactiveStatusListValue = inactiveListValueId,
      }
    );

    _settingsMock
    .SetupGet(m => m.Azure)
    .Returns(
      new AzureSettings
      {
        OnspringActiveGroups = new string[]
        {
          "1",
        },
      }
    );

    var result = _onspringService.GetUsersStatus(
      azureUser,
      onspringUser,
      userGroupIds
    );

    result.Should().NotBeNull();
    result.Should().BeOfType<StringFieldValue>();
    result!.FieldId.Should().Be(1);
    result.AsString().Should().Be(inactiveListValueId.ToString());
  }
}