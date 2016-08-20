using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.Features.Event;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.UnitTest.Controllers
{
    public class EventApiControllerTests
    {
        [Fact]
        public void GetSendsEventsWithUnlockedCampaignsQuery()
        {
            var mediator = new Mock<IMediator>();
            var sut = new EventApiController(mediator.Object, null);
            sut.Get();

            mediator.Verify(x => x.Send(It.IsAny<EventsWithUnlockedCampaignsQuery>()), Times.Once);
        }

        [Fact]
        public void GetReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsWithUnlockedCampaignsQuery>())).Returns(new List<EventViewModel>());

            var sut = new EventApiController(mediator.Object, null);
            var results = sut.Get();

            Assert.IsType<List<EventViewModel>>(results);
        }

        [Fact]
        public void GetHasHttpGetAttribute()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Get()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void GetByIdSendsEventByEventIdQueryWithCorrectData()
        {
            const int eventId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });
            var sut = new EventApiController(mediator.Object, null);

            sut.Get(eventId);

            mediator.Verify(x => x.Send(It.Is<EventByIdQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public void GetByIdReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });
            var sut = new EventApiController(mediator.Object, null);
            var result = sut.Get(It.IsAny<int>());

            Assert.IsType<EventViewModel>(result);
        }

        //TODO: come back to these two tests until you hear back from Tony Surma about returning null instead of retruning HttpNotFound
        //GetByIdReturnsNullWhenEventIsNotFoundById ???
        //[Fact]
        //public void GetByIdReturnsHttpNotFoundWhenEventIsNotFoundById()
        //{
        //    var controller = new EventApiController(Mock.Of<IAllReadyDataAccess>(), null)
        //        .SetFakeUser("1");

        //    var result = controller.Get(It.IsAny<int>());
        //    Assert.IsType<NotFoundResult>(result);
        //}

        [Fact]
        public void GetByIdHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}");
        }

        [Fact]
        public void GetByIdHasProducesAttributeWithCorrectContentTypes()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Type, typeof(EventViewModel));
            Assert.Equal(attribute.ContentTypes.Select(x => x).First(), "application/json");
        }

        [Fact]
        public void GetEventsByDateRangeHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByDateRange(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{start}/{end}");
        }

        [Fact]
        public void GetEventsByDateRangeHasProducesAttribute()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByDateRange(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes[0], "application/json");
            Assert.Equal(attribute.Type, typeof(EventViewModel));
        }

        [Fact]
        public void GetEventsByDateRangeSendsEventByDateRangeQueryWithCorrectDates()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByDateRangeQuery>())).Returns(new List<EventViewModel>());

            var sut = new EventApiController(mediator.Object, null);
            sut.GetEventsByDateRange(may, june);

            mediator.Verify(x => x.Send(It.Is<EventByDateRangeQuery>(y => y.StartDate == may && y.EndDate == june)), Times.Once);
        }


        [Fact]
        public void GetEventsByDateRangeReturnsNoContentWhenEventsIsNull()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByDateRangeQuery>())).Returns((List<EventViewModel>)null);

            var sut = new EventApiController(mediator.Object, null);
            var result = sut.GetEventsByDateRange(may, june);

            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public void GetEventsByDateRangeReturnsJsonWhenEventsIsNotNull()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByDateRangeQuery>())).Returns(new List<EventViewModel>());

            var sut = new EventApiController(mediator.Object, null);
            var result = sut.GetEventsByDateRange(may, june);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void GetEventsByPostalCodeSendsEventsByPostalCodeQueryWithCorrectPostalCodeAndDistance()
        {
            const string zip = "zip";
            const int miles = 100;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByPostalCodeQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object, null);
            sut.GetEventsByPostalCode(zip, miles);

            mediator.Verify(x => x.Send(It.Is<EventsByPostalCodeQuery>(y => y.PostalCode == zip && y.Distance == miles)), Times.Once);
        }

        [Fact]
        public void GetEventsByPostalCodeReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByPostalCodeQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object, null);
            var result = sut.GetEventsByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            Assert.IsType<List<EventViewModel>>(result);
        }

        [Fact]
        public void GetEventsByPostalCodeHasRouteAttributeWithRoute()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByPostalCode(It.IsAny<string>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "search");
        }

        [Fact]
        public void GetEventsByGeographySendsEventsByGeographyQueryWithCorrectLatitudeLongitudeAndMiles()
        {
            const double latitude = 1;
            const double longitude = 2;
            const int miles = 100;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByGeographyQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object, null);
            sut.GetEventsByGeography(latitude, longitude, miles);

            mediator.Verify(x => x.Send(It.Is<EventsByGeographyQuery>(y => y.Latitude == latitude && y.Longitude == longitude && y.Miles == miles)), Times.Once);
        }

        [Fact]
        public void GetEventsByGeographyReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByGeographyQuery>())).Returns(new List<Event>());

            var sut = new EventApiController(mediator.Object, null);
            var result = sut.GetEventsByGeography(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>());

            Assert.IsType<List<EventViewModel>>(result);
        }

        [Fact]
        public void GetEventsByLocationHasRouteAttributeWithCorrectRoute()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetEventsByGeography(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "searchbylocation");
        }

        [Fact]
        public void GetQrCodeHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetQrCode(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/qrcode");
        }

        [Fact]
        public void GetCheckinReturnsHttpNotFoundWhenUnableToFindEventByEventId()
        {
            var sut = new EventApiController(Mock.Of<IMediator>(), null);
            var result = sut.GetCheckin(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetCheckinReturnsTheCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });

            var sut = new EventApiController(mediator.Object, null);
            var result = (ViewResult)sut.GetCheckin(It.IsAny<int>());

            Assert.IsType<Event>(result.ViewData.Model);
        }

        [Fact]
        public void GetCheckinReturnsTheCorrectView()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganization = new Organization() } });

            var sut = new EventApiController(mediator.Object, null);
            var result = (ViewResult)sut.GetCheckin(It.IsAny<int>());

            Assert.Equal("NoUserCheckin", result.ViewName);
        }

        [Fact]
        public void GetCheckinHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetCheckin(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/checkin");
        }

        [Fact]
        public async Task PutCheckinReturnsHttpNotFoundWhenUnableToFindEventByEventId()
        {
            var sut = new EventApiController(Mock.Of<IMediator>(), null);
            var result = await sut.PutCheckin(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutCheckinSendsEventByEventIdQueryWithCorrectEventId()
        {
            const int eventId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event());

            var sut = new EventApiController(mediator.Object, null);
            await sut.PutCheckin(eventId);

            mediator.Verify(x => x.Send(It.Is<EventByIdQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task PutCheckinSendsAddEventSignupCommandAsyncWithCorrectDataWhenUsersSignedUpIsNotNullAndCheckinDateTimeIsNull()
        {
            const string userId = "userId";
            var utcNow = DateTime.UtcNow;

            var campaignEvent = new Event();
            var eventSignup = new EventSignup { User = new ApplicationUser { Id = userId } };
            campaignEvent.UsersSignedUp.Add(eventSignup);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(campaignEvent);

            var sut = new EventApiController(mediator.Object, null) { DateTimeUtcNow = () => utcNow };
            sut.SetFakeUser(userId);
            await sut.PutCheckin(It.IsAny<int>());

            mediator.Verify(x => x.SendAsync(It.Is<AddEventSignupCommandAsync>(y => y.EventSignup == eventSignup)));
            mediator.Verify(x => x.SendAsync(It.Is<AddEventSignupCommandAsync>(y => y.EventSignup.CheckinDateTime == utcNow)));
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task PutCheckinReturnsCorrectJsonWhenUsersSignedUpIsNotNullAndCheckinDateTimeIsNull()
        {
            const string userId = "userId";

            var campaignEvent = new Event { Name = "EventName", Description = "EventDescription" };
            var eventSignup = new EventSignup { User = new ApplicationUser { Id = userId } };
            campaignEvent.UsersSignedUp.Add(eventSignup);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(campaignEvent);

            var sut = new EventApiController(mediator.Object, null);
            sut.SetFakeUser(userId);

            var expected = $"{{ Event = {{ Name = {campaignEvent.Name}, Description = {campaignEvent.Description} }} }}";

            var result = (JsonResult)await sut.PutCheckin(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, result.Value.ToString());
        }

        [Fact]
        public async Task PutCheckinReturnsCorrectJsonWhenUsersSignedUpIsNullAndCheckinDateTimeIsNotNull()
        {
            const string userId = "userId";
            var campaignEvent = new Event { Name = "EventName", Description = "EventDescription" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(campaignEvent);

            var sut = new EventApiController(mediator.Object, null);
            sut.SetFakeUser(userId);

            var expected = $"{{ NeedsSignup = True, Event = {{ Name = {campaignEvent.Name}, Description = {campaignEvent.Description} }} }}";

            var result = (JsonResult)await sut.PutCheckin(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, result.Value.ToString());
        }

        [Fact]
        public void PutCheckinHasHttpPutAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null, null);
            var attribute = (HttpPutAttribute)sut.GetAttributesOn(x => x.PutCheckin(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(HttpPutAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/checkin");
        }

        [Fact]
        public void PutCheckinHasAuthorizeAttribute()
        {
            var sut = new EventApiController(null, null);
            var attribute = (AuthorizeAttribute)sut.GetAttributesOn(x => x.PutCheckin(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(AuthorizeAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RegisterEventReturnsHttpBadRequetWhenSignupModelIsNull()
        {
            var sut = new EventApiController(null, null);
            var result = await sut.RegisterEvent(null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RegisterEventReturnsCorrectJsonWhenModelStateIsNotValid()
        {
            const string modelStateErrorMessage = "modelStateErrorMessage";

            var sut = new EventApiController(null, null);
            sut.AddModelStateErrorWithErrorMessage(modelStateErrorMessage);

            var jsonResult = (JsonResult)await sut.RegisterEvent(new EventSignupViewModel());
            var result = jsonResult.GetValueForProperty<List<string>>("errors");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<List<string>>(result);
            Assert.Equal(result.First(), modelStateErrorMessage);
        }

        [Fact]
        public async Task RegisterEventSendsEventSignupCommandAsyncWithCorrectData()
        {
            var model = new EventSignupViewModel();
            var mediator = new Mock<IMediator>();

            var sut = new EventApiController(mediator.Object, null);
            await sut.RegisterEvent(model);

            mediator.Verify(x => x.SendAsync(It.Is<EventSignupCommand>(command => command.EventSignup.Equals(model))));
        }

        [Fact]
        public async Task RegisterEventReturnsSuccess()
        {
            var sut = new EventApiController(Mock.Of<IMediator>(), null);
            var result = await sut.RegisterEvent(new EventSignupViewModel());

            Assert.True(result.ToString().Contains("success"));
        }

        [Fact]
        public void RegisterEventHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new EventApiController(null, null);
            var attribute = (ValidateAntiForgeryTokenAttribute)sut.GetAttributesOn(x => x.RegisterEvent(It.IsAny<EventSignupViewModel>())).SingleOrDefault(x => x.GetType() == typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterEventHasHttpPostAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null, null);
            var attribute = (HttpPostAttribute)sut.GetAttributesOn(x => x.RegisterEvent(It.IsAny<EventSignupViewModel>())).SingleOrDefault(x => x.GetType() == typeof(HttpPostAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "signup");
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UnregisterEventReturnsHttpNotFoundWhenUnableToGetEventSignupByEventSignupIdAndUserId()
        {
            var controller = new EventApiController(Mock.Of<IMediator>(), null);
            controller.SetDefaultHttpContext();

            var result = await controller.UnregisterEvent(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UnregisterEventSendsEventSignupByEventIdAndUserIdQueryWithCorrectEventIdAndUserId()
        {
            const int eventId = 1;
            const string userId = "1";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventSignupByEventIdAndUserIdQuery>()))
                .Returns(new EventSignup { Event = new Event(), User = new ApplicationUser() });

            var controller = new EventApiController(mediator.Object, null);
            controller.SetFakeUser(userId);

            await controller.UnregisterEvent(eventId);

            mediator.Verify(x => x.Send(It.Is<EventSignupByEventIdAndUserIdQuery>(y => y.EventId == eventId && y.UserId == userId)));
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UnregisterEventSendsUnregisterEventWithCorrectEventSignupId()
        {
            const int eventId = 1;
            const int eventSignupId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventSignupByEventIdAndUserIdQuery>()))
                .Returns(new EventSignup { Id = eventSignupId, Event = new Event(), User = new ApplicationUser() });

            var controller = new EventApiController(mediator.Object, null);
            controller.SetDefaultHttpContext();

            await controller.UnregisterEvent(eventId);

            mediator.Verify(x => x.SendAsync(It.Is<UnregisterEvent>(y => y.EventSignupId == eventSignupId)));
        }

        [Fact]
        public void UnregisterEventHasHttpDeleteAttributeWithCorrectTemplate()
        {
            var sut = new EventApiController(null, null);
            var attribute = (HttpDeleteAttribute)sut.GetAttributesOn(x => x.UnregisterEvent(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(HttpDeleteAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/signup");
        }

        [Fact]
        public void UnregisterEventHasAuthorizeAttribute()
        {
            var sut = new EventApiController(null, null);
            var attribute = (AuthorizeAttribute)sut.GetAttributesOn(x => x.UnregisterEvent(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(AuthorizeAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/event");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new EventApiController(null, null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x).First(), "application/json");
        }
    }
}