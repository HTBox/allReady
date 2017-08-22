using System.Collections.Generic;
using System.Linq;
using AllReady.Controllers;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels;
using AllReady.ViewModels.Event;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class CampaignApiControllerTests
    {
        [Fact]
        public void GetCampaignsByPostalCodeReturnsCorrectResults()
        {
            var event1 = new Event { Id = 1, CampaignId = 1 };
            event1.Campaign = new Campaign { Id = 1, Events = new List<Event> { event1 }, ManagingOrganization = new Organization() };

            var event2 = new Event { Id = 2 };
            var event3 = new Event { Id = 3 };
            var campaign2 = new Campaign { Id = 2, Events = new List<Event> { event2, event3 }, ManagingOrganization = new Organization() };

            event2.CampaignId = campaign2.Id;
            event2.Campaign = campaign2;

            event3.CampaignId = campaign2.Id;
            event3.Campaign = campaign2;

            var allEvents = new List<Event> { event1, event2, event3 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByPostalCodeQuery>())).Returns(allEvents);

            var sut = new CampaignApiController(mediator.Object);
            var results = sut.GetCampaignsByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            Assert.Equal(results.Count(), allEvents.Count);
        }

        [Fact]
        public void GetCampaignsByPostalCodeSendsEventsByPostalCodeQueryWithCorrectPostalCodeAndDistance()
        {
            const string postalCode = "postecode";
            const int miles = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByPostalCodeQuery>())).Returns(new List<Event>());

            var sut = new CampaignApiController(mediator.Object);
            sut.GetCampaignsByPostalCode(postalCode, miles);

            mediator.Verify(x => x.Send(It.Is<EventsByPostalCodeQuery>(y => y.PostalCode == postalCode && y.Distance == miles)));
        }
        
        [Fact]
        public void GetCampaignsByPostalCodeReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventsByPostalCodeQuery>())).Returns(new List<Event>());

            var sut = new CampaignApiController(mediator.Object);
            var result = sut.GetCampaignsByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            Assert.IsType<List<EventViewModel>>(result);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new CampaignApiController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("api/campaign", attribute.Template);
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new CampaignApiController(null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }
    }
}
