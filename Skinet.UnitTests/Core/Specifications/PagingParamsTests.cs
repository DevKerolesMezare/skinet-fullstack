using Core.Specifications;

namespace Skinet.UnitTests.Core.Specifications;

[TestFixture]
public class PagingParamsTests
{
    [Test]
    public void PageSize_WhenExceedsMaximum_ShouldBeCappedAt50()
    {
        var parameters = new PagingParams
        {
            PageSize = 100
        };

        Assert.That(parameters.PageSize, Is.EqualTo(50));
    }


    [Test]
    public void Constructor_ShouldSetDefaultPagingValues()
    {
        var parameters = new PagingParams();

        Assert.Multiple(() =>
        {
            Assert.That(parameters.PageIndex, Is.EqualTo(1));
            Assert.That(parameters.PageSize, Is.EqualTo(6));
        });
    }

    [Test]
    public void PageSize_WhenWithinMaximum_ShouldKeepAssignedValue()
    {
        var parameters = new PagingParams
        {
            PageSize = 10
        };

        Assert.That(parameters.PageSize, Is.EqualTo(10));
    }

}