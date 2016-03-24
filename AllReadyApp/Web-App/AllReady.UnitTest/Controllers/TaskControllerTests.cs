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
using Microsoft.AspNet.Authorization;

namespace AllReady.UnitTest.Controllers
{
    public class TaskControllerTests
    {
        [Fact]
        public void CreateGetInvokesGetActivityWithTheCorrectActivityId()
        {
            const int activityId = 1;
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new TaskController(dataAccess.Object, null);
            sut.Create(activityId);

            dataAccess.Verify(x => x.GetActivity(activityId), Times.Once);
        }

        [Fact]
        public void CreateGetReturnsHttpUnauthorizedResultWhenActivityIsNull()
        {
            var sut = new TaskController(Mock.Of<IAllReadyDataAccess>(), null);
            var result = sut.Create(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public void CreateGetReturnsHttpUnauthorizedResultWhenUserIsNotOrganizationAdminForTheirOrganization()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivity(It.IsAny<int>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganizationId = 1 } });

            var sut = new TaskController(dataAccess.Object, null);
            sut.SetDefaultHttpContext();

            var result = sut.Create(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public void CreateGetReturnsCorrectViewModel()
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

            var claims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(dataAccess.Object, null);
            PopulateClaimsFor(sut, claims);

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
        public void CreateGetReturnsCorrectView()
        {
            const int organizationId = 1;

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivity(It.IsAny<int>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganizationId = organizationId }});

            var claims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(dataAccess.Object, null);
            PopulateClaimsFor(sut, claims);

            var result = sut.Create(It.IsAny<int>()) as ViewResult;

            Assert.Equal(result.ViewName, "Edit");
        }

        [Fact]
        public void CreateGetHasHttpGetAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<TaskEditModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateGetHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<TaskEditModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Create/{activityId}");
        }

        [Fact]
        public void CreatePost()
        {
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAuthorizeAtttributeWithTheCorrectPolicy()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        private static void PopulateClaimsFor(Controller controller, IEnumerable<Claim> claims)
        {
            var httpContext = new Mock<HttpContext>();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.Setup(x => x.User).Returns(claimsPrincipal);

            controller.ActionContext.HttpContext = httpContext.Object;
        }        
    }
}
