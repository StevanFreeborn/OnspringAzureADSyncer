namespace OnspringAzureADSyncer.Models;

public class OnspringSettings
{
  public string BaseUrl { get; init; } = string.Empty;
  public string ApiKey { get; init; } = string.Empty;
  public int UsersAppId { get; init; }
  public int GroupsAppId { get; init; }
  public int GroupsNameFieldId { get; init; }
  public int GroupsDescriptionFieldId { get; init; }
}