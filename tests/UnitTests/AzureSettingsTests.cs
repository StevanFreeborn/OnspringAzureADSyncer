using GroupFilter = OnspringAzureADSyncer.Models.GroupFilter;

namespace OnspringAzureADSyncerTests.UnitTests;

public class AzureSettingsTests
{
  [Fact]
  public void AzureSettings_ItShouldHaveProperConstantPropertyValues()
  {
    AzureSettings.GroupsNameKey.Should().Be("id");
    AzureSettings.GroupsDescriptionKey.Should().Be("description");
    AzureSettings.UsersIdPropertyKey.Should().Be("id");
    AzureSettings.UsersUsernameKey.Should().Be("userPrincipalName");
    AzureSettings.UsersFirstNameKey.Should().Be("givenName");
    AzureSettings.UsersLastNameKey.Should().Be("surname");
    AzureSettings.UsersEmailKey.Should().Be("mail");
    AzureSettings.UsersStatusKey.Should().Be("accountEnabled");
    AzureSettings.UsersGroupsKey.Should().Be("memberOf");
  }

  [Fact]
  public void AzureSettings_WhenConstructed_ItShouldHaveDefaultPropertyValues()
  {
    var settings = new AzureSettings();
    settings.TenantId.Should().BeEmpty();
    settings.ClientId.Should().BeEmpty();
    settings.ClientSecret.Should().BeEmpty();
    settings.OnspringActiveGroups.Should().BeEmpty();
    settings.UsersProperties.Should().BeEquivalentTo(typeof(User).GetProperties());
    settings.GroupsProperties.Should().BeEquivalentTo(typeof(Group).GetProperties());
    settings.GroupFilter.Should().BeEmpty();
  }

  [Fact]
  public void AzureSettings_WhenInitialized_ItShouldBeAbleToSetItsProperties()
  {
    var settings = new AzureSettings
    {
      TenantId = "tenantId",
      ClientId = "clientId",
      ClientSecret = "clientSecret",
      OnspringActiveGroups = ["group1", "group2"],
      GroupFilter = "displayName eq 'Onspring Users'"
    };

    settings.TenantId.Should().Be("tenantId");
    settings.ClientId.Should().Be("clientId");
    settings.ClientSecret.Should().Be("clientSecret");
    settings.OnspringActiveGroups.Should().BeEquivalentTo(["group1", "group2"]);
    settings.GroupFilter.Should().Be("displayName eq 'Onspring Users'");
  }
}