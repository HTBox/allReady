using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationByIdQueryHandlerShould
    {
        [Fact]
        public void InvokeGetOrganizationWithCorrectOrganizationId()
        {
            var message = new OrganizationByIdQuery { OrganizationId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new OrganizationByIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetOrganization(message.OrganizationId), Times.Once);
        }
    }
}
