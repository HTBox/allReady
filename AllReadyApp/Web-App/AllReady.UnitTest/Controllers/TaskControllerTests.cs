using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Models;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;
using Microsoft.AspNet.Http;
using System.Security.Claims;
using AllReady.UnitTest.Extensions;
using AllReady.Areas.Admin.Models;

namespace AllReady.UnitTest.Controllers
{
    public class TaskControllerTests
    {
        [Fact]
        public void CreateInvokesGetActivityWithTheCorrectActivityId()
        {
            const int activityId = 1;
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new TaskController(dataAccess.Object, null);
            sut.Create(activityId);

            dataAccess.Verify(x => x.GetActivity(activityId), Times.Once);
        }

        [Fact]
        public void CreateReturnsHttpUnauthorizedResultWhenActivityIsNull()
        {
            var sut = new TaskController(Mock.Of<IAllReadyDataAccess>(), null);
            var result = sut.Create(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public void CreateReturnsHttpUnauthorizedResultWhenUserIsNotOrganizationAdminForTheirOrganization()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivity(It.IsAny<int>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganizationId = 1 } });

            var sut = new TaskController(dataAccess.Object, null);
            sut.SetDefaultHttpContext();

            var result = sut.Create(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public void CreateReturnsCorrectViewModel()
        {
            const int organizationId = 1;
            const int campaignId = 1;
            var campaignStartDateTime = DateTime.Now.AddDays(-1);
            var campaignEndDateTime = DateTime.Now.AddDays(1);

            var activity = new Activity
            {
                Id = 1,
                Name = "ActivityName",
                CampaignId = campaignId,
                Campaign = new Campaign
                {
                    Id = campaignId,
                    Name = "CampaignName",
                    ManagingOrganizationId = organizationId,
                    TimeZoneId = "EST",
                    StartDateTime = campaignStartDateTime,
                    EndDateTime = campaignEndDateTime
                }
            };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivity(It.IsAny<int>())).Returns(activity);

            var httpContext = new Mock<HttpContext>();
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            }));
            httpContext.Setup(x => x.User).Returns(claimsPrincipal);

            var sut = new TaskController(dataAccess.Object, null);
            sut.ActionContext.HttpContext = httpContext.Object;

            var result = sut.Create(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskEditModel;

            Assert.Equal(modelResult.ActivityId, activity.Id);
            Assert.Equal(modelResult.ActivityName, activity.Name);
            Assert.Equal(modelResult.CampaignId, activity.CampaignId);
            Assert.Equal(modelResult.CampaignName, activity.Campaign.Name);
            Assert.Equal(modelResult.OrganizationId, activity.Campaign.ManagingOrganizationId);
            Assert.Equal(modelResult.TimeZoneId, activity.Campaign.TimeZoneId);
            Assert.Equal(modelResult.StartDateTime, activity.StartDateTime);
            Assert.Equal(modelResult.EndDateTime, activity.EndDateTime);
        }

        [Fact]
        public void CreateReturnsCorrectView()
        {
            const int organizationId = 1;

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivity(It.IsAny<int>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganizationId = organizationId }});

            var httpContext = new Mock<HttpContext>();
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            }));
            httpContext.Setup(x => x.User).Returns(claimsPrincipal);

            var sut = new TaskController(dataAccess.Object, null);
            sut.ActionContext.HttpContext = httpContext.Object;

            var result = sut.Create(It.IsAny<int>()) as ViewResult;

            Assert.Equal(result.ViewName, "Edit");
        }
    }
}
