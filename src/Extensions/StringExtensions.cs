namespace OnspringAzureADSyncer.Extensions;

public static class StringExtensions
{
  public static string Capitalize(this string word)
  {
    return word[..1].ToUpper() + word[1..].ToLower();
  }
}