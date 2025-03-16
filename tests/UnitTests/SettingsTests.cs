namespace OnspringAzureADSyncerTests.UnitTests;

public class SettingsTests
{
  private readonly Mock<IOptions<AppOptions>> _appOptionsMock;

  public SettingsTests()
  {
    _appOptionsMock = new Mock<IOptions<AppOptions>>();
  }

  [Fact]
  public void Settings_ItShouldHaveConstantPropertyForSettingsSection()
  {
    Settings.SettingsSection.Should().Be("Settings");
  }

  [Fact]
  public void Settings_ItShouldHaveDefaultGroupsFieldMappingsBasedOnConstantPropertiesOfAzureSettingsAndOnspringSettings()
  {
    Settings
      .DefaultGroupsFieldMappings
      .Should()
      .BeEquivalentTo(new Dictionary<string, string>
      {
        {
          AzureSettings.GroupsNameKey,
          OnspringSettings.GroupsNameField
        },
        {
          AzureSettings.GroupsDescriptionKey,
          OnspringSettings.GroupsDescriptionField
        }
      });
  }

  [Fact]
  public void Settings_WhenConstructed_ItShouldHaveDefaultPropertyValues()
  {
    var filePath = Path.Combine(
      Directory.GetCurrentDirectory(),
      "testData",
      "testconfig.json"
    );

    _appOptionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = new FileInfo(filePath),
        LogLevel = LogEventLevel.Information
      });

    var settings = new Settings(_appOptionsMock.Object);
    settings.Azure.Should().NotBeNull();
    settings.Onspring.Should().NotBeNull();
    settings.GroupsFieldMappings.Should().NotBeNull();
    settings.UsersFieldMappings.Should().NotBeNull();
    settings.UsersFieldMappings.Should().BeEmpty();
    settings.GroupsFieldMappings.Should().BeEmpty();
  }

  [Fact]
  public void Settings_WhenInitialized_ItShouldBeAbleToSetItsProperties()
  {
    var filePath = Path.Combine(
      Directory.GetCurrentDirectory(),
      "testData",
      "testconfig.json"
    );

    _appOptionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = new FileInfo(filePath),
        LogLevel = LogEventLevel.Information
      });

    var azureSettings = new AzureSettings
    {
      ClientId = "ClientId",
      ClientSecret = "ClientSecret",
      TenantId = "TenantId"
    };

    var onspringSettings = new OnspringSettings
    {
      ApiKey = "ApiKey",
      BaseUrl = "https://api.onspring.com",
      UsersAppId = 1,
      GroupsAppId = 2
    };

    var groupFieldMappings = new Dictionary<int, string>
    {
      {
        1,
        "id"
      },
      {
        2,
        "description"
      }
    };

    var usersFieldMappings = new Dictionary<int, string>
    {
      {
        1,
        "id"
      },
      {
        2,
        "description"
      }
    };

    var settings = new Settings(_appOptionsMock.Object)
    {
      Azure = azureSettings,
      Onspring = onspringSettings,
      GroupsFieldMappings = groupFieldMappings,
      UsersFieldMappings = usersFieldMappings,
    };

    settings.Azure.Should().Be(azureSettings);
    settings.Onspring.Should().Be(onspringSettings);
    settings.GroupsFieldMappings.Should().BeEquivalentTo(groupFieldMappings);
    settings.UsersFieldMappings.Should().BeEquivalentTo(usersFieldMappings);
  }

  [Fact]
  public void GetMappedUserPropertiesAsCamelCase_WhenCalled_ItShouldReturnMappedPropertiesAsCamelCase()
  {
    var filePath = Path.Combine(
      Directory.GetCurrentDirectory(),
      "testData",
      "testconfig.json"
    );

    _appOptionsMock
      .SetupGet(static x => x.Value)
      .Returns(new AppOptions
      {
        ConfigFile = new FileInfo(filePath),
        LogLevel = LogEventLevel.Information
      });

    var settings = new Settings(_appOptionsMock.Object)
    {
      UsersFieldMappings = new Dictionary<int, string>
      {
        {
          1,
          "id"
        },
        {
          2,
          "DisplayName"
        },
        {
          3,
          "description"
        },
        {
          4,
          "streetaddress"
        }
      }
    };

    var mappedProperties = settings.GetMappedUserPropertiesAsCamelCase();

    mappedProperties.Should().BeEquivalentTo(new List<string>
    {
      "id",
      "displayName",
      "streetAddress"
    });
  }
}