using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.ViewModels.OrganizationApi;
using AllReady.Constants;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationContactQuery>(y => y.OrganizationId == organizationId && y.ContactType == ContactTypes.Primary)));
        }

        [Fact]
        public async Task GetContactReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationContactQuery>())).ReturnsAsync(new ContactInformationViewModel());

            var sut = new OrganizationApiController(mediator.Object);
            var result = await sut.GetContact(It.IsAny<int>());

            Assert.IsType<ContactInformationViewModel>(result);
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
            Assert.Equal(typeof(ContactInformationViewModel), attribute.Type);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("admin/api/organization", attribute.Template);
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(AreaNames.Admin, attribute.RouteValue);
        }

        [Fact]
        public void ControllerHasAuthorizeAtttributeWithCorrectPolicy()
        {
            var sut = new OrganizationApiController(null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("OrgAdmin", attribute.Policy);
        }
    }
}
