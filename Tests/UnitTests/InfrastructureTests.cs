using NUnit.Framework;

namespace MTM.Tests.UnitTests;

/// <summary>
/// Basic test to verify test infrastructure is working
/// </summary>
[TestFixture]
[NUnit.Framework.Category("Unit")]
public class InfrastructureTests
{
    [Test]
    public void TestFramework_ShouldWork()
    {
        // Arrange & Act & Assert
        Assert.That(true, Is.True, "Test framework should be working");
    }

    [Test]
    public void BasicMath_ShouldWork()
    {
        // Arrange
        int a = 2;
        int b = 3;

        // Act
        int result = a + b;

        // Assert
        Assert.That(result, Is.EqualTo(5), "Basic math should work correctly");
    }

    [Test]
    [TestCase("TEST001", "100", 25)]
    [TestCase("PART002", "110", 50)]
    public void ParameterizedTest_ShouldWork(string partId, string operation, int quantity)
    {
        // Assert
        Assert.That(partId, Is.Not.Empty, "Part ID should not be empty");
        Assert.That(operation, Is.Not.Empty, "Operation should not be empty");
        Assert.That(quantity, Is.GreaterThan(0), "Quantity should be greater than 0");
    }
}