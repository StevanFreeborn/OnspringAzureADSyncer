using GroupFilter = OnspringAzureADSyncer.Models.GroupFilter;

namespace OnspringAzureADSyncerTests.UnitTests;

public class GroupFilterTests
{
  [Theory]
  [InlineData("DisplayName", @"^Test.*", true)]
  [InlineData("", @"^Test.*", false)]
  [InlineData("InvalidProperty", @"^Test.*", false)]
  [InlineData("DisplayName", "", false)]
  [InlineData("DisplayName", " ", false)]
  [InlineData("DisplayName", "[", false)]
  [InlineData("Photos", @"^Test.*", false)]
  public void IsValid_ShouldValidatePropertyAndPattern(string property, string pattern, bool expectedResult)
  {
    var filter = new GroupFilter
    {
      Property = property,
      Pattern = pattern
    };

    var result = filter.IsValid();

    result.Should().Be(expectedResult);
  }

  [Fact]
  public void IsMatch_WhenCalledWithValidPropertyAndPattern_ItShouldMatchCorrectly()
  {
    var filter = new GroupFilter
    {
      Property = "DisplayName",
      Pattern = "^Test"
    };

    var group = new Group
    {
      DisplayName = "Test Group"
    };

    var result = filter.IsMatch(group);

    result.Should().BeTrue();
  }

  [Fact]
  public void IsMatch_WhenCalledWithNonMatchingPattern_ItShouldReturnFalse()
  {
    var filter = new GroupFilter
    {
      Property = "DisplayName",
      Pattern = "^Admin"
    };

    var group = new Group
    {
      DisplayName = "Test Group"
    };

    var result = filter.IsMatch(group);

    result.Should().BeFalse();
  }

  [Fact]
  public void IsMatch_WhenCalledWithInvalidProperty_ItShouldReturnFalse()
  {
    var filter = new GroupFilter
    {
      Property = "NonExistentProperty",
      Pattern = ".*"
    };

    var group = new Group
    {
      DisplayName = "Test Group"
    };

    var result = filter.IsMatch(group);

    result.Should().BeFalse();
  }

  [Fact]
  public void IsMatch_WhenCalledWithNullPropertyValue_ItShouldReturnFalse()
  {
    var filter = new GroupFilter
    {
      Property = "DisplayName",
      Pattern = ".*"
    };

    var group = new Group
    {
      DisplayName = null
    };

    var result = filter.IsMatch(group);

    result.Should().BeFalse();
  }

  [Fact]
  public void IsMatch_WhenCalledWithComplexRegexPattern_ItShouldHandleTimeoutGracefully()
  {
    var filter = new GroupFilter
    {
      Property = "DisplayName",
      Pattern = @"^(a?){50}$" // Pattern known to cause catastrophic backtracking
    };

    var group = new Group
    {
      DisplayName = new string('a', 100)
    };

    var result = filter.IsMatch(group);

    result.Should().BeFalse(); // Should return false due to timeout
  }

  [Fact]
  public void IsMatch_WhenCalledWithCaseInsensitiveProperty_ItShouldMatchCorrectly()
  {
    var filter = new GroupFilter
    {
      Property = "displayname",
      Pattern = "^Test"
    };

    var group = new Group
    {
      DisplayName = "Test Group"
    };

    var result = filter.IsMatch(group);

    result.Should().BeTrue();
  }
}