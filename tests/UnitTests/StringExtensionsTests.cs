namespace OnspringAzureADSyncerTests.UnitTests;

public class StringExtensionsTests
{
  [Theory]
  [InlineData("hello", "Hello")]
  [InlineData("HELLO", "HELLO")]
  public void Capitalize(string value, string expectedResult)
  {
    var actual = value.Capitalize();
    actual.Should().Be(expectedResult);
  }
}