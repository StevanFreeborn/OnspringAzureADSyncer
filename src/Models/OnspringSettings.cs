namespace OnspringAzureADSyncer.Models;

public class OnspringSettings
{
  public const string GroupsNameField = "Name";
  public const string GroupsDescriptionField = "Description";
  public const string UsersUsernameField = "Username";
  public const string UsersFirstNameField = "First Name";
  public const string UsersLastNameField = "Last Name";
  public const string UsersEmailField = "Email Address";
  public const string UsersStatusField = "Status";
  public const string UsersActiveStatusListValueName = "Active";
  public const string UsersInactiveStatusListValueName = "Inactive";
  public const string UsersUserTierField = "User Tier";
  public const string UsersFullUserTierListValueName = "Full User";
  public const string UsersLiteUserTierListValueName = "Lite User";
  public const string UsersPortalUserTierListValueName = "Portal User";
  public const string UsersGroupsField = "Groups";

  public string BaseUrl { get; init; } = string.Empty;
  public string ApiKey { get; init; } = string.Empty;
  public int UsersAppId { get; init; }
  public int GroupsAppId { get; init; }
  public List<Field> UsersFields { get; set; } = [];
  public List<Field> GroupsFields { get; set; } = [];
  public int GroupsNameFieldId { get; set; }
  public int UsersUsernameFieldId { get; set; }
  public int UsersStatusFieldId { get; set; }
  public int UsersGroupsFieldId { get; set; }
  public int UsersUserTierFieldId { get; set; }
  public Guid UserActiveStatusListValue { get; set; } = Guid.Empty;
  public Guid UserInactiveStatusListValue { get; set; } = Guid.Empty;
  public Guid UserFullUserTierListValue { get; set; } = Guid.Empty;
  public Guid UserLiteUserTierListValue { get; set; } = Guid.Empty;
  public Guid UserPortalUserTierListValue { get; set; } = Guid.Empty;

  public string[] GroupRequiredFields { get; } = [
    GroupsNameField,
  ];

  public string[] UserRequiredFields { get; } = [
    UsersUsernameField,
    UsersFirstNameField,
    UsersLastNameField,
    UsersEmailField,
  ];
}