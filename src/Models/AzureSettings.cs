namespace OnspringAzureADSyncer.Models;

class AzureSettings
{
  public const string GroupsNameKey = "id";
  public const string GroupsDescriptionKey = "description";
  public string TenantId { get; init; } = string.Empty;
  public string ClientId { get; init; } = string.Empty;
  public string ClientSecret { get; init; } = string.Empty;
}