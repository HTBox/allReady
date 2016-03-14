using System.Collections.Generic;
using System.Linq;
using AllReady.Controllers;
using AllReady.Features.Activity;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class CampaignApiControllerTests
    {
        [Fact]
        public void GetCampaignsByPostalCodeReturnsCorrectResults()
        {
            var activity1 = new Activity { Id = 1, CampaignId = 1 };
            activity1.Campaign = new Campaign { Id = 1, Activities = new List<Activity> { activity1 }, ManagingOrganization = new Organization() };

            var activity2 = new Activity { Id = 2 };
            var activity3 = new Activity { Id = 3 };
            var campaign2 = new Campaign { Id = 2, Activities = new List<Activity> { activity2, activity3 }, ManagingOrganization = new Organization() };

            activity2.CampaignId = campaign2.Id;
            activity2.Campaign = campaign2;

            activity3.CampaignId = campaign2.Id;
            activity3.Campaign = campaign2;

            var allActivities = new List<Activity> { activity1, activity2, activity3 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AcitivitiesByPostalCodeQuery>())).Returns(allActivities);

            var sut = new CampaignApiController(mediator.Object);
            var results = sut.GetCampaignsByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            Assert.Equal(results.Count(), allActivities.Count);
        }

        [Fact]
        public void GetCampaignsByPostalCodeSendsAcitivitiesByPostalCodeQueryWithCorrectPostalCodeAndDistance()
        {
            const string zip = "zip";
            const int miles = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AcitivitiesByPostalCodeQuery>())).Returns(new List<Activity>());

            var sut = new CampaignApiController(mediator.Object);
            sut.GetCampaignsByPostalCode(zip, miles);

            mediator.Verify(x => x.Send(It.Is<AcitivitiesByPostalCodeQuery>(y => y.PostalCode == zip && y.Distance == miles)));
        }
        
        [Fact]
        public void GetCampaignsByPostalCodeReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AcitivitiesByPostalCodeQuery>())).Returns(new List<Activity>());

            var sut = new CampaignApiController(mediator.Object);
            var result = sut.GetCampaignsByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            Assert.IsType<List<ActivityViewModel>>(result);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new CampaignApiController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/campaign");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new CampaignApiController(null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }
    }
}
