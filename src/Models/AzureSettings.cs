using Group = Microsoft.Graph.Models.Group;

namespace OnspringAzureADSyncer.Models;

public class AzureSettings
{
  public const string GroupsNameKey = "id";
  public const string GroupsDescriptionKey = "description";
  public const string UsersIdPropertyKey = "id";
  public const string UsersUsernameKey = "userPrincipalName";
  public const string UsersFirstNameKey = "givenName";
  public const string UsersLastNameKey = "surname";
  public const string UsersEmailKey = "mail";
  public const string UsersStatusKey = "accountEnabled";
  public const string UsersGroupsKey = "memberOf";
  public string TenantId { get; init; } = string.Empty;
  public string ClientId { get; init; } = string.Empty;
  public string ClientSecret { get; init; } = string.Empty;
  public string[] OnspringActiveGroups { get; init; } = [];
  public PropertyInfo[] UsersProperties { get; } = typeof(User).GetProperties();
  public PropertyInfo[] GroupsProperties { get; } = typeof(Group).GetProperties();
  public string GroupFilter { get; init; } = string.Empty;
}