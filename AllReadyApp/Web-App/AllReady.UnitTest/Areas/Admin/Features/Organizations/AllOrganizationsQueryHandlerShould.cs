using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class AllOrganizationsQueryHandlerShould
    {
        [Fact]
        public void InvokeOrganizations()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new AllOrganizationsQueryHandler(dataAccess.Object);
            sut.Handle(new AllOrganizationsQuery());

            dataAccess.Verify(x => x.Organizations, Times.Once);
        }
    }
}
