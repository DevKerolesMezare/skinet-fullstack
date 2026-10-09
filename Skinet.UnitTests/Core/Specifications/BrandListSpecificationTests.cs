using Core.Specifications;

namespace Skinet.UnitTests.Core.Specifications;

[TestFixture]
public class BrandListSpecificationTests
{
    [Test]
    public void Constructor_ShouldSelectBrandAndApplyDistinct()
    {
        // Arrange
        var spec = new BrandListSpecification();

        // Assert
        Assert.That(spec.Select, Is.Not.Null);
        Assert.That(spec.IsDistinct, Is.True);
    }
}
