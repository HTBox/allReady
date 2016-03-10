using System.Collections.Generic;
using System.Linq;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels;
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
            //three activities, two should have same campaign
            //making the assumption that the Campaign on the Campaign property on Activity is should have the same Id as the CampaignId property on the same Activity instance
            var activity2 = new Activity { Id = 2 };
            var activity3 = new Activity { Id = 3 };
            var campaign2 = new Campaign { Id = 2, ManagingOrganization = new Organization(), Activities = new List<Activity> { activity2, activity3 }};

            var activity1 = new Activity { Id = 1 };
            activity1.CampaignId = 1;
            activity1.Campaign = new Campaign { Id = 1, ManagingOrganization = new Organization(), Activities = new List<Activity> { activity1 } };

            activity2.CampaignId = campaign2.Id;
            activity2.Campaign = campaign2;

            activity3.CampaignId= campaign2.Id;
            activity3.Campaign = campaign2;

            //why does the Activity class carry both a Campaign and a CampaignId? When would onen be set to a valid instance and the other is not???
            var allActivities = new List<Activity> { activity1, activity2, activity3 };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.ActivitiesByPostalCode(It.IsAny<string>(), It.IsAny<int>())).Returns(allActivities);

            var sut = new CampaignApiController(dataAccess.Object);
            var results = sut.GetCampaignsByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            //TODO: figure out what to assert here
        }

        [Fact]
        public void GetCampaignsByPostalCodeInvokesActivitiesByPostalCodeWithCorrectPostalCodeAndDistance()
        {
            const string zip = "zip";
            const int miles = 1;

            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new CampaignApiController(dataAccess.Object);
            sut.GetCampaignsByPostalCode(zip, miles);

            dataAccess.Verify(x => x.ActivitiesByPostalCode(zip, miles));
        }
        
        [Fact]
        public void GetCampaignsByPostalCodeReturnsCorrectModel()
        {
            var sut = new CampaignApiController(Mock.Of<IAllReadyDataAccess>());
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
