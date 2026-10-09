
using Core.Specifications;


namespace Skinet.UnitTests.Core.Specifications;

[TestFixture]
public class BaseSpecificationTests
{
    private class TestSpecification : BaseSpecification<int>
    {
        public TestSpecification() : base()
        {
        }
    }

    [Test]
    public void ApplayCriteria_WhenCriteriaIsNull_ShouldReturnAllItems()
    {
        var specification = new TestSpecification();
        var items = new[] { 1, 2, 3 }.AsQueryable();

        var result = specification.ApplayCriteria(items);

        Assert.That(result, Is.EqualTo(items));
    }
}
