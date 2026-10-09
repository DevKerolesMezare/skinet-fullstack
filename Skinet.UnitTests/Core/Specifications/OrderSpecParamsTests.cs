using Core.Specifications;

namespace Skinet.UnitTests.Core.Specifications;

[TestFixture]
public class OrderSpecParamsTests
{
    [Test]
    public void Status_WhenAssigned_ShouldReturnAssignedValue()
    {
        var parameters = new OrderSpecParams
        {
            Status = "PaymentReceived"
        };

        Assert.That(parameters.Status, Is.EqualTo("PaymentReceived"));
    }
}
