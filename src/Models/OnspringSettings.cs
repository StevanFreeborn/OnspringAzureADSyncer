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
  public const string UsersGroupsField = "Groups";
  public const string UsersActiveStatusListValueName = "Active";
  public const string UsersInactiveStatusListValueName = "Inactive";
  public string BaseUrl { get; init; } = string.Empty;
  public string ApiKey { get; init; } = string.Empty;
  public int UsersAppId { get; init; } = 0;
  public int GroupsAppId { get; init; } = 0;
  public List<Field> UsersFields { get; set; } = new List<Field>();
  public List<Field> GroupsFields { get; set; } = new List<Field>();
  public int UsersUsernameFieldId { get; set; } = 0;
  public int UsersStatusFieldId { get; set; } = 0;
  public Guid UserActiveStatusListValue { get; set; }
  public Guid UserInactiveStatusListValue { get; set; }
}