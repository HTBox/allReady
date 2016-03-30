using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class OrganizationApiControllerTests
    {
        [Fact]
        public async Task GetContactSendsOrganizationContactQueryWithCorrectData()
        {
            const int organizationId = 1;
            var mediator = new Mock<IMediator>();

            var sut = new OrganizationApiController(mediator.Object);
            await sut.GetContact(organizationId);

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationContactQueryAsync>(y => y.OrganizationId == organizationId && y.ContactType == ContactTypes.Primary)));
        }

        [Fact]
        public async Task GetContactReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationContactQueryAsync>())).ReturnsAsync(new ContactInformationModel());

            var sut = new OrganizationApiController(mediator.Object);
            var result = await sut.GetContact(It.IsAny<int>());

            Assert.IsType<ContactInformationModel>(result);
        }

        [Fact]
        public void RegisterTaskHasHttpGetAttribute()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetContact(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterTaskHasProducesAttributeWithCorrectContentType()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetContact(It.IsAny<int>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Type, typeof (ContactInformationModel));
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "admin/api/organization");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectRouteValue()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAuthorizeAtttributeWithCorrectPolicy()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }
    }
}
