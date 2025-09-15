using NUnit.Framework;
using FluentAssertions;

namespace MTM.Tests.UnitTests.Framework;

/// <summary>
/// Basic test to verify testing framework setup and demonstrate foundation
/// </summary>
[TestFixture]
[NUnit.Framework.Category("Unit")]
[NUnit.Framework.Category("Framework")]
public class TestingFrameworkTests
{
    [Test]
    public void TestFramework_NUnit_ShouldWork()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert - Using NUnit assertions
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestFramework_FluentAssertions_ShouldWork()
    {
        // Arrange
        var numbers = new[] { 1, 2, 3, 4, 5 };

        // Act & Assert - Using FluentAssertions
        numbers.Should().NotBeEmpty();
        numbers.Should().HaveCount(5);
        numbers.Should().Contain(3);
        numbers[0].Should().Be(1);
    }

    [Test]
    [TestCase(1, 2, 3)]
    [TestCase(5, 10, 15)]
    [TestCase(-1, -2, -3)]
    public void TestFramework_Parameterized_ShouldWork(int a, int b, int expected)
    {
        // Act
        var result = a + b;

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void TestFramework_Categories_ShouldAllowFiltering()
    {
        // This test demonstrates category-based test filtering
        // Can be run with: dotnet test --filter "Category=Framework"
        
        // Assert
        true.Should().BeTrue("Test framework categories should work for filtering");
    }
}