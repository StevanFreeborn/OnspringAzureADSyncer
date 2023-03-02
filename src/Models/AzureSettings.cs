namespace OnspringAzureADSyncer.Models;

public class AzureSettings
{
  public string TenantId { get; init; } = string.Empty;
  public string ClientId { get; init; } = string.Empty;
  public string ClientSecret { get; init; } = string.Empty;
}