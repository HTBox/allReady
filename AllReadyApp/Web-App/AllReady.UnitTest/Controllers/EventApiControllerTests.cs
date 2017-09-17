using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.Features.Events;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Event;
using MediatR;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.UnitTest.Controllers
{
    public class EventApiControllerTests
    {
        [Fact]
        public async Task GetSendsEventsWithUnlockedCampaignsQuery()
        {
            var mediator = new Mock<IMediator>();
            var sut = new EventApiController(mediator.Object);
            await sut.Get();

            mediator.Verify(x => x.SendAsync(It.IsAny<EventsWithUnlockedCampaignsQuery>()), Times.Once);
        }

        [Fact]
        public async Task GetReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventsWithUnlockedCampaignsQuery>())).ReturnsAsync(new List<EventViewModel>());

            var sut = new EventApiController(mediator.Object);
            var results = await sut.Get();

            Assert.IsType<List<EventViewModel>>(results);
        }

        [Fact]
        public void GetHasHttpGetAttribute()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.Get()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task GetByIdSendsEventByEventIdQueryWithCorrectData()
        {
            const int eventId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });
            var sut = new EventApiController(mediator.Object);

            await sut.Get(eventId);

            mediator.Verify(x => x.SendAsync(It.Is<EventByEventIdQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public async Task GetByIdReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });
            var sut = new EventApiController(mediator.Object);
            var result = await sut.Get(It.IsAny<int>());

            Assert.IsType<EventViewModel>(result);
        }

        [Fact]
        public void GetByIdHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("{id}", attribute.Template);
        }

        [Fact]
        public void GetByIdHasProducesAttributeWithCorrectContentTypes()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(typeof(EventViewModel), attribute.Type);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }

        [Fact]
        public void GetEventsByDateRangeHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByDateRange(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("{start}/{end}", attribute.Template);
        }

        [Fact]
        public void GetEventsByDateRangeHasProducesAttribute()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByDateRange(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("application/json", attribute.ContentTypes[0]);
            Assert.Equal(typeof(EventViewModel), attribute.Type);
        }

        [Fact]
        public async Task GetEventsByDateRangeSendsEventByDateRangeQueryWithCorrectDates()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByDateRangeQuery>())).ReturnsAsync(new List<EventViewModel>());

            var sut = new EventApiController(mediator.Object);
            await sut.GetEventsByDateRange(may, june);

            mediator.Verify(x => x.SendAsync(It.Is<EventByDateRangeQuery>(y => y.StartDate == may && y.EndDate == june)), Times.Once);
        }


        [Fact]
        public async Task GetEventsByDateRangeReturnsNoContentWhenEventsIsNull()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByDateRangeQuery>())).ReturnsAsync((List<EventViewModel>)null);

            var sut = new EventApiController(mediator.Object);
            var result = await sut.GetEventsByDateRange(may, june);

            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task GetEventsByDateRangeReturnsJsonWhenEventsIsNotNull()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByDateRangeQuery>())).ReturnsAsync(new List<EventViewModel>());

            var sut = new EventApiController(mediator.Object);
            var result = await sut.GetEventsByDateRange(may, june);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void GetEventsByPostalCodeSendsEventsByPostalCodeQueryWithCorrectPostalCodeAndDistance()
        {
            const string postalCode = "postalcode";
            const int miles = 100;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByPostalCodeQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object);
            sut.GetEventsByPostalCode(postalCode, miles);

            mediator.Verify(x => x.Send(It.Is<EventsByPostalCodeQuery>(y => y.PostalCode == postalCode && y.Distance == miles)), Times.Once);
        }

        [Fact]
        public void GetEventsByPostalCodeReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByPostalCodeQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object);
            var result = sut.GetEventsByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            Assert.IsType<List<EventViewModel>>(result);
        }

        [Fact]
        public void GetEventsByPostalCodeHasRouteAttributeWithRoute()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByPostalCode(It.IsAny<string>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("search", attribute.Template);
        }

        [Fact]
        public void GetEventsByGeographySendsEventsByGeographyQueryWithCorrectLatitudeLongitudeAndMiles()
        {
            const double latitude = 1;
            const double longitude = 2;
            const int miles = 100;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByGeographyQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object);
            sut.GetEventsByGeography(latitude, longitude, miles);

            mediator.Verify(x => x.Send(It.Is<EventsByGeographyQuery>(y => y.Latitude == latitude && y.Longitude == longitude && y.Miles == miles)), Times.Once);
        }

        [Fact]
        public void GetEventsByGeographyReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByGeographyQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object);
            var result = sut.GetEventsByGeography(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>());

            Assert.IsType<List<EventViewModel>>(result);
        }

        [Fact]
        public void GetEventsByLocationHasRouteAttributeWithCorrectRoute()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByGeography(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("searchbylocation", attribute.Template);
        }


        [Fact]
        public async Task GetCheckinReturnsHttpNotFoundWhenUnableToFindEventByEventId()
        {
            var sut = new EventApiController(Mock.Of<IMediator>());
            var result = await sut.GetCheckin(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCheckinReturnsTheCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });

            var sut = new EventApiController(mediator.Object);
            var result = await sut.GetCheckin(It.IsAny<int>()) as ViewResult;

            Assert.IsType<Event>(result.ViewData.Model);
        }

        [Fact]
        public async Task GetCheckinReturnsTheCorrectView()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });

            var sut = new EventApiController(mediator.Object);
            var result = await sut.GetCheckin(It.IsAny<int>()) as ViewResult;

            Assert.Equal("NoUserCheckin", result.ViewName);
        }

        [Fact]
        public void GetCheckinHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetCheckin(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("{id}/checkin", attribute.Template);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("api/event", attribute.Template);
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new EventApiController(null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }
    }
}