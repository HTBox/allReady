using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Controllers;
using AllReady.Models;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;
using Microsoft.AspNet.Http;
using System.Security.Claims;
using AllReady.UnitTest.Extensions;
using AllReady.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNet.Authorization;
using AllReady.Areas.Admin.Features.Tasks;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Controllers
{
    //TODO:
    //- encapsulate orgAdmimClaims into private method
    //- move PopulateClaimsFor to ControllerTestHelper?
    //- break out WarnDateTimeOutOfRange to separate testable entity?
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

            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(dataAccess.Object, null);
            PopulateClaimsFor(sut, orgAdminClaims);

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

            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(dataAccess.Object, null);
            PopulateClaimsFor(sut, orgAdminClaims);

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

        //TODO: come back to these b/c of WarnDateTimeOutOfRange
        [Fact]
        public void CreatePost()
        {
        }

        [Fact]
        public async Task EditGetSendsEditTaskQueryWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();

            var sut = new TaskController(null, mediator.Object);
            await sut.Edit(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<EditTaskQuery>(y => y.TaskId == taskId)));
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var sut = new TaskController(null, Mock.Of<IMediator>());
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task EditGetReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditTaskQuery>())).ReturnsAsync(new TaskEditModel());

            var sut = new TaskController(null, mediator.Object);
            sut.SetDefaultHttpContext();
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewModelAndView()
        {
            const int organizationId = 1;
            var taskEditModel = new TaskEditModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditTaskQuery>())).ReturnsAsync(taskEditModel);

            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(null, mediator.Object);
            PopulateClaimsFor(sut, orgAdminClaims);

            var result = await sut.Edit(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskEditModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<TaskEditModel>(modelResult);
            Assert.Equal(modelResult, taskEditModel);
        }

        [Fact]
        public void EditGetHasHttpGetAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditGetHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Edit/{id}");
        }

        //TODO: come back to testing because of WarnDateTimeOutOfRange
        //[Fact]
        //public void EditPostAddsModelStateErrorWhenEndDateTimeIsLessThanStartDateTime()
        //{
        //    var sut = new TaskController(null, null);
        //    sut.Edit(new TaskEditModel { EndDateTime = DateTimeOffset.MinValue, StartDateTime = DateTimeOffset.MaxValue });
        //    var errorMessage = sut.ModelState.GetErrorMessages().First();
        //    Assert.Equal(errorMessage, "Ending time cannot be earlier than the starting time");
        //}

        //[Fact]
        //public void EditPost()
        //{
        //}

        [Fact]
        public async Task DeleteSendsTaskQueryWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(null, mediator.Object);
            await sut.Delete(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQuery>(y => y.TaskId == taskId)));
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var sut = new TaskController(null, Mock.Of<IMediator>());
            var result = await sut.Delete(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQuery>())).ReturnsAsync(new TaskSummaryModel());

            var sut = new TaskController(null, mediator.Object);
            sut.SetDefaultHttpContext();
            var result = await sut.Delete(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewModelAndView()
        {
            const int organizationId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQuery>())).ReturnsAsync(taskSummaryModel);

            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(null, mediator.Object);
            PopulateClaimsFor(sut, orgAdminClaims);

            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskSummaryModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<TaskSummaryModel>(modelResult);
            Assert.Equal(modelResult, taskSummaryModel);
        }

        [Fact]
        public async Task DetailsSendsTaskQueryWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(null, mediator.Object);
            await sut.Details(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQuery>(y => y.TaskId == taskId)));
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var sut = new TaskController(null, Mock.Of<IMediator>());
            var result = await sut.Details(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModelAndView()
        {
            var taskSummaryModel = new TaskSummaryModel();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQuery>())).ReturnsAsync(taskSummaryModel);

            var sut = new TaskController(null, mediator.Object);
            var result = await sut.Details(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskSummaryModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<TaskSummaryModel>(modelResult);
            Assert.Equal(modelResult, taskSummaryModel);
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Details/{id}");
        }

        [Fact]
        public async Task DeleteConfirmedSendsTaskQueryWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(null, mediator.Object);
            await sut.DeleteConfirmed(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQuery>(y => y.TaskId == taskId)));
        }

        [Fact]
        public async Task DeleteConfirmedReturnsHttpNotFoundResultWhenTaskSummaryModelIsNull()
        {
            var sut = new TaskController(null, Mock.Of<IMediator>());
            var result = await sut.DeleteConfirmed(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQuery>())).ReturnsAsync(new TaskSummaryModel());

            var sut = new TaskController(null, mediator.Object);
            sut.SetDefaultHttpContext();

            var result = await sut.DeleteConfirmed(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmedSendsDeleteTaskCommandWithCorrectTaskId()
        {
            const int organizationId = 1;
            const int taskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQuery>())).ReturnsAsync(new TaskSummaryModel { OrganizationId = 1 });

            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(null, mediator.Object);
            PopulateClaimsFor(sut, orgAdminClaims);
            await sut.DeleteConfirmed(taskId);

            mediator.Verify(x => x.Send(It.Is<DeleteTaskCommand>(y => y.TaskId == taskId)));
        }

        [Fact]
        public async Task DeleteConfirmedRedirectsToAction()
        {
            const int organizationId = 1;
            const int taskId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId, ActivityId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQuery>())).ReturnsAsync(taskSummaryModel);

            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            };

            var sut = new TaskController(null, mediator.Object);
            PopulateClaimsFor(sut, orgAdminClaims);
            var result = await sut.DeleteConfirmed(taskId) as RedirectToActionResult;

            var routeValues = new Dictionary<string, object> { ["id"] = taskSummaryModel.ActivityId };

            Assert.Equal(result.ActionName, nameof(ActivityController.Details));
            Assert.Equal(result.ControllerName, "Activity");
            Assert.Equal(result.RouteValues, routeValues);
        }

        //[Fact]
        //public void AssignSendsTaskQueryWithCorrectTaskId()
        //{
        //    var mediator = new Mock<IMediator>();
        //    var sut = new TaskController(null, mediator.Object);
        //    sut
        //}

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
