using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;
using System.Linq;
using Microsoft.AspNet.Authorization;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Models.ItineraryModels;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ItineraryAdminControllerTests
    {
        [Fact]
        public async Task DetailsSendsEventDetailQueryAsyncWithCorrectEventId()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new ItineraryController(mockMediator.Object);
            await sut.Details(1);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>()), Times.Once);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenEventIsNull()
        {
            ItineraryController controller;
            MockMediatorItineraryDetailQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var sut = GetItineraryControllerWithDetailsQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModelWhenEventIsNotNullAndUserIsOrgAdmin()
        {
            var sut = GetItineraryControllerWithDetailsQuery(UserType.OrgAdmin.ToString(), 1);
            Assert.IsType<ViewResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModelWhenEventIsNotNullAndUserIsSiteAdmin()
        {
            var sut = GetItineraryControllerWithDetailsQuery(UserType.SiteAdmin.ToString(), 0);
            Assert.IsType<ViewResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>());
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>());
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Details/{id}");
        }
        
        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>());
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAreaAuthorizeAttributeWithCorrectPolicy()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>());
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        #region Helper Methods
        private static Mock<IMediator> MockMediatorItineraryDetailQuery(out ItineraryController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            controller = new ItineraryController(mockMediator.Object);

            return mockMediator;
        }

        private static ItineraryController GetItineraryControllerWithDetailsQuery(string userType, int organizationId)
        {
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(new ItineraryDetailsModel { Id = 1, Name = "Itinerary", EventId = 1, EventName = "Event Name", OrganizationId = 1, Date = new DateTime(2016, 07, 01) });

            var sut = new ItineraryController(mediator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return sut;
        }      

        #endregion
    }
}