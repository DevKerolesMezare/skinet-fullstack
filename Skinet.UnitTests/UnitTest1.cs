namespace Skinet.UnitTests;

[TestFixture]
public class Tests
{
    [Test]
    public void Add_TwoNumbers_ReturnsSum()
    {
        // Arrange
        int a = 2;
        int b = 3;

        // Act
        int result = a + b;

        // Assert
        Assert.That(result, Is.EqualTo(5));
    }
}
