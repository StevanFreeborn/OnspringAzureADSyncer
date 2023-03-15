namespace OnspringAzureADSyncerTests.UnitTests;

public class OnspringSettingsTests
{
  [Fact]
  public void OnspringSettings_ItShouldHaveProperConstantPropertyValues()
  {
    OnspringSettings.GroupsNameField.Should().Be("Name");
    OnspringSettings.GroupsDescriptionField.Should().Be("Description");
    OnspringSettings.UsersUsernameField.Should().Be("Username");
    OnspringSettings.UsersFirstNameField.Should().Be("First Name");
    OnspringSettings.UsersLastNameField.Should().Be("Last Name");
    OnspringSettings.UsersEmailField.Should().Be("Email Address");
    OnspringSettings.UsersStatusField.Should().Be("Status");
    OnspringSettings.UsersGroupsField.Should().Be("Groups");
    OnspringSettings.UsersActiveStatusListValueName.Should().Be("Active");
    OnspringSettings.UsersInactiveStatusListValueName.Should().Be("Inactive");
  }

  [Fact]
  public void OnspringSettings_WhenConstructed_ItShouldHaveDefaultPropertyValues()
  {
    var settings = new OnspringSettings();
    settings.ApiKey.Should().BeEmpty();
    settings.BaseUrl.Should().BeEmpty();
    settings.UsersAppId.Should().Be(0);
    settings.GroupsAppId.Should().Be(0);
    settings.UsersFields.Should().BeEmpty();
    settings.GroupsFields.Should().BeEmpty();
    settings.GroupsNameFieldId.Should().Be(0);
    settings.UsersUsernameFieldId.Should().Be(0);
    settings.UsersStatusFieldId.Should().Be(0);
    settings.UsersGroupsFieldId.Should().Be(0);
    settings.UserActiveStatusListValue.Should().Be(Guid.Empty);
    settings.UserInactiveStatusListValue.Should().Be(Guid.Empty);
  }

  [Fact]
  public void OnspringSettings_WhenInitialized_ItShouldBeAbleToSetItsProperties()
  {
    var settings = new OnspringSettings
    {
      ApiKey = "ApiKey",
      BaseUrl = "https://api.onspring.com",
      UsersAppId = 1,
      GroupsAppId = 2,
      UsersFields = new List<Field>(),
      GroupsFields = new List<Field>(),
      GroupsNameFieldId = 3,
      UsersUsernameFieldId = 4,
      UsersStatusFieldId = 5,
      UsersGroupsFieldId = 6,
      UserActiveStatusListValue = Guid.NewGuid(),
      UserInactiveStatusListValue = Guid.NewGuid()
    };

    settings.ApiKey.Should().Be("ApiKey");
    settings.BaseUrl.Should().Be("https://api.onspring.com");
    settings.UsersAppId.Should().Be(1);
    settings.GroupsAppId.Should().Be(2);
    settings.UsersFields.Should().BeEmpty();
    settings.GroupsFields.Should().BeEmpty();
    settings.GroupsNameFieldId.Should().Be(3);
    settings.UsersUsernameFieldId.Should().Be(4);
    settings.UsersStatusFieldId.Should().Be(5);
    settings.UsersGroupsFieldId.Should().Be(6);
    settings.UserActiveStatusListValue.Should().NotBe(Guid.Empty);
    settings.UserInactiveStatusListValue.Should().NotBe(Guid.Empty);
  }
}