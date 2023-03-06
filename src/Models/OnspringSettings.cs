namespace OnspringAzureADSyncer.Models;

public class OnspringSettings
{
  public const string GroupsNameField = "Name";
  public const string GroupsDescriptionField = "Description";
  public string BaseUrl { get; init; } = string.Empty;
  public string ApiKey { get; init; } = string.Empty;
  public int UsersAppId { get; init; } = 0;
  public int GroupsAppId { get; init; } = 0;
}