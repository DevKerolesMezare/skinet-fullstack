using Core.Specifications;

namespace Skinet.UnitTests.Core.Specifications;


[TestFixture]
public class TypeListSpecificationTests
{
    [Test]
    public void Constructor_ShouldSelectTypeAndApplyDistinct()
    {
        var spec = new TypeListSpecification();

        Assert.Multiple(() =>
        {
            Assert.That(spec.Select, Is.Not.Null);
            Assert.That(spec.IsDistinct, Is.True);
        });
    }
}