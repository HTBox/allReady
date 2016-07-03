using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Controllers;
using AllReady.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AllReady.UnitTest.Extensions;
using AllReady.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using AllReady.Areas.Admin.Features.Tasks;
using System.Threading.Tasks;
using AllReady.Features.Event;
using AllReady.Extensions;

namespace AllReady.UnitTest.Controllers
{
    public class TaskControllerTests
    {
        [Fact]
        public void CreateGetSendsEventByEventIdQueryWithTheCorrectEventId()
        {
            const int eventId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(mediator.Object);
            sut.Create(eventId);

            mediator.Verify(x => x.Send(It.Is<EventByIdQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public void CreateGetReturnsHttpUnauthorizedResultWhenEventIsNull()
        {
            var sut = new TaskController(Mock.Of<IMediator>());
            var result = sut.Create(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void CreateGetReturnsHttpUnauthorizedResultWhenUserIsNotOrganizationAdminForTheirOrganization()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = 1 }});

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();

            var result = sut.Create(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void CreateGetReturnsCorrectViewModel()
        {
            const int organizationId = 1;
            const int campaignId = 1;
            var campaignStartDateTime = DateTime.Now.AddDays(-1);
            var campaignEndDateTime = DateTime.Now.AddDays(1);

            var campaignEvent = new Event
            {
                Id = 1,
                Name = "EventName",
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

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(campaignEvent);

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            var result = sut.Create(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskEditModel;

            Assert.Equal(modelResult.EventId, campaignEvent.Id);
            Assert.Equal(modelResult.EventName, campaignEvent.Name);
            Assert.Equal(modelResult.CampaignId, campaignEvent.CampaignId);
            Assert.Equal(modelResult.CampaignName, campaignEvent.Campaign.Name);
            Assert.Equal(modelResult.OrganizationId, campaignEvent.Campaign.ManagingOrganizationId);
            Assert.Equal(modelResult.TimeZoneId, campaignEvent.Campaign.TimeZoneId);
            Assert.Equal(modelResult.StartDateTime, campaignEvent.StartDateTime);
            Assert.Equal(modelResult.EndDateTime, campaignEvent.EndDateTime);
        }

        [Fact]
        public void CreateGetReturnsCorrectView()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = organizationId }});

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            var result = sut.Create(It.IsAny<int>()) as ViewResult;

            Assert.Equal(result.ViewName, "Edit");
        }

        [Fact]
        public void CreateGetHasHttpGetAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateGetHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Create/{eventId}");
        }

        [Fact]
        public async Task CreatePostAddsCorrectModelStateErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var mediator = new Mock<IMediator>();
            //mgmccarthy: for a list of time zone's to feed to TimeZoneInfo.FindSystemTimeZoneById, check here:
            //http://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" } });

            var sut = new TaskController(mediator.Object);
            await sut.Create(It.IsAny<int>(), new TaskEditModel { EndDateTime = DateTimeOffset.Now.AddDays(-1), StartDateTime = DateTimeOffset.Now.AddDays(1) });

            var modelStateErrorCollection = sut.ModelState.GetErrorMessagesByKey(nameof(TaskEditModel.EndDateTime));
            Assert.Equal(modelStateErrorCollection.Single().ErrorMessage, "Ending time cannot be earlier than the starting time");
        }

        [Fact]
        public async Task CreatePostReturnsCorrectViewAndViewModelWhenEndDateTimeIsLessThanStartDateTimeAndModelStateIsInvalid()
        {
            var model = new TaskEditModel { EndDateTime = DateTimeOffset.Now.AddDays(-1), StartDateTime = DateTimeOffset.Now.AddDays(1) };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" } });

            var sut = new TaskController(mediator.Object);
            var result = await sut.Create(It.IsAny<int>(), model) as ViewResult;
            var modelResult = result.ViewData.Model as TaskEditModel;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(modelResult, model);
        }

        [Fact]
        public async Task CreatePostReturnsHttpUnauthorizedResultWhenModelStateIsValidAndUserIsNotAnOrganizationAdminUser()
        {
            var startDateTime = DateTimeOffset.Now.AddDays(-1);
            var endDateTime = DateTimeOffset.Now.AddDays(1);
            var model = new TaskEditModel { StartDateTime = startDateTime, EndDateTime = endDateTime, OrganizationId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" },
                StartDateTime = startDateTime.AddDays(-1),
                EndDateTime = endDateTime.AddDays(1)
            });

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();

            var result = await sut.Create(It.IsAny<int>(), model);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CreatePostSendsEditTaskCommandAsyncWhenModelStateIsValidAndUserIsOrganizationAdmin()
        {
            const int organizationId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId };
            var startDateTime = DateTimeOffset.Now.AddDays(-1);
            var endDateTime = DateTimeOffset.Now.AddDays(1);

            var model = new TaskEditModel { StartDateTime = startDateTime, EndDateTime = endDateTime, OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" },
                StartDateTime = startDateTime.AddDays(-1),
                EndDateTime = endDateTime.AddDays(1)
            });
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskSummaryModel);

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            await sut.Create(It.IsAny<int>(), model);

            mediator.Verify(x => x.SendAsync(It.Is<EditTaskCommandAsync>(y => y.Task == model)));
        }

        [Fact]
        public async Task CreatePostRedirectsToCorrectAction()
        {
            const int organizationId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId };
            var startDateTime = DateTimeOffset.Now.AddDays(-1);
            var endDateTime = DateTimeOffset.Now.AddDays(1);
            const int taskEditModelId = 1;
            var model = new TaskEditModel { Id = taskEditModelId, StartDateTime = startDateTime, EndDateTime = endDateTime, OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" },
                StartDateTime = startDateTime.AddDays(-1),
                EndDateTime = endDateTime.AddDays(1)
            });
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskSummaryModel);

            var routeValues = new Dictionary<string, object> { ["id"] = model.Id };

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            var result = await sut.Create(taskEditModelId, model) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(TaskController.Details));
            Assert.Equal(result.ControllerName, "Event");
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void CreatePostHasHttpGetAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<TaskEditModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreatePostHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<TaskEditModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Create/{eventId}");
        }

        [Fact]
        public async Task EditGetSendsEditTaskQueryAsyncWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();

            var sut = new TaskController(mediator.Object);
            await sut.Edit(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<EditTaskQueryAsync>(y => y.TaskId == taskId)), Times.Once);
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var sut = new TaskController(Mock.Of<IMediator>());
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditGetReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditTaskQueryAsync>())).ReturnsAsync(new TaskEditModel());

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewModelAndView()
        {
            const int organizationId = 1;
            var taskEditModel = new TaskEditModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditTaskQueryAsync>())).ReturnsAsync(taskEditModel);

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            var result = await sut.Edit(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskEditModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<TaskEditModel>(modelResult);
            Assert.Equal(modelResult, taskEditModel);
        }

        [Fact]
        public void EditGetHasHttpGetAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditGetHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Edit/{id}");
        }

        [Fact]
        public async Task EditPostAddsCorrectModelStateErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" } });

            var sut = new TaskController(mediator.Object);
            await sut.Edit(new TaskEditModel { EndDateTime = DateTimeOffset.Now.AddDays(-1), StartDateTime = DateTimeOffset.Now.AddDays(1)});

            var modelStateErrorCollection = sut.ModelState.GetErrorMessagesByKey(nameof(TaskEditModel.EndDateTime));
            Assert.Equal(modelStateErrorCollection.Single().ErrorMessage, "Ending time cannot be earlier than the starting time");
        }

        [Fact]
        public async Task EditPostReturnsCorrectViewAndViewModelWhenEndDateTimeIsLessThanStartDateTimeAndModelStateIsInvalid()
        {
            var model = new TaskEditModel { EndDateTime = DateTimeOffset.Now.AddDays(-1), StartDateTime = DateTimeOffset.Now.AddDays(1) };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" } });

            var sut = new TaskController(mediator.Object);
            var result = await sut.Edit(model) as ViewResult;
            var modelResult = result.ViewData.Model as TaskEditModel;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(modelResult, model);
        }

        [Fact]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenModelStateIsValidAndUserIsNotAnOrganizationAdminUser()
        {
            var startDateTime = DateTimeOffset.Now.AddDays(-1);
            var endDateTime = DateTimeOffset.Now.AddDays(1);
            var model = new TaskEditModel { StartDateTime = startDateTime, EndDateTime = endDateTime, OrganizationId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" },
                StartDateTime = startDateTime.AddDays(-1),
                EndDateTime = endDateTime.AddDays(1)
            });

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();

            var result = await sut.Edit(model);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditPostSendsEditTaskCommandAsyncWhenModelStateIsValidAndUserIsOrganizationAdmin()
        {
            const int organizationId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId };
            var startDateTime = DateTimeOffset.Now.AddDays(-1);
            var endDateTime = DateTimeOffset.Now.AddDays(1);

            var model = new TaskEditModel { StartDateTime = startDateTime, EndDateTime = endDateTime, OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" }, StartDateTime = startDateTime.AddDays(-1), EndDateTime = endDateTime.AddDays(1)
            });
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskSummaryModel);

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            await sut.Edit(model);

            mediator.Verify(x => x.SendAsync(It.Is<EditTaskCommandAsync>(y => y.Task == model)));
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectAction()
        {
            const int organizationId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId };
            var startDateTime = DateTimeOffset.Now.AddDays(-1);
            var endDateTime = DateTimeOffset.Now.AddDays(1);
            var model = new TaskEditModel { Id = 1, StartDateTime = startDateTime, EndDateTime = endDateTime, OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Campaign = new Campaign { TimeZoneId = "Eastern Standard Time" },
                StartDateTime = startDateTime.AddDays(-1),
                EndDateTime = endDateTime.AddDays(1)
            });
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskSummaryModel);

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            var result = await sut.Edit(model) as RedirectToActionResult;

            var routeValues = new Dictionary<string, object>{ ["id"] = model.Id };

            Assert.Equal(result.ControllerName, "Task");
            Assert.Equal(result.ActionName, nameof(TaskController.Details));
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void EditPostHasHttpPostAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<TaskEditModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<TaskEditModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task DeleteSendsTaskQueryAsyncWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(mediator.Object);
            await sut.Delete(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQueryAsync>(y => y.TaskId == taskId)), Times.Once);
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var sut = new TaskController(Mock.Of<IMediator>());
            var result = await sut.Delete(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(new TaskSummaryModel());

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();
            var result = await sut.Delete(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewModelAndView()
        {
            const int organizationId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskSummaryModel);

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskSummaryModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<TaskSummaryModel>(modelResult);
            Assert.Equal(modelResult, taskSummaryModel);
        }

        [Fact]
        public async Task DetailsSendsTaskQueryAsyncWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(mediator.Object);
            await sut.Details(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQueryAsync>(y => y.TaskId == taskId)), Times.Once);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var sut = new TaskController(Mock.Of<IMediator>());
            var result = await sut.Details(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModelAndView()
        {
            var taskSummaryModel = new TaskSummaryModel();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskSummaryModel);

            var sut = new TaskController(mediator.Object);
            var result = await sut.Details(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as TaskSummaryModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<TaskSummaryModel>(modelResult);
            Assert.Equal(modelResult, taskSummaryModel);
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Details/{id}");
        }

        [Fact]
        public async Task DeleteConfirmedSendsTaskQueryAsyncWithCorrectTaskId()
        {
            const int taskId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(mediator.Object);
            await sut.DeleteConfirmed(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQueryAsync>(y => y.TaskId == taskId)), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmedReturnsHttpNotFoundResultWhenTaskSummaryModelIsNull()
        {
            var sut = new TaskController(Mock.Of<IMediator>());
            var result = await sut.DeleteConfirmed(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(new TaskSummaryModel());

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();

            var result = await sut.DeleteConfirmed(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmedSendsDeleteTaskCommandAsyncWithCorrectTaskId()
        {
            const int organizationId = 1;
            const int taskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(new TaskSummaryModel { OrganizationId = 1 });

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());
            await sut.DeleteConfirmed(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<DeleteTaskCommandAsync>(y => y.TaskId == taskId)), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmedRedirectsToCorrectAction()
        {
            const int organizationId = 1;
            const int taskId = 1;
            var taskSummaryModel = new TaskSummaryModel { OrganizationId = organizationId, EventId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskSummaryModel);

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());
            var result = await sut.DeleteConfirmed(taskId) as RedirectToActionResult;

            var routeValues = new Dictionary<string, object> { ["id"] = taskSummaryModel.EventId };

            Assert.Equal(result.ActionName, nameof(EventController.Details));
            Assert.Equal(result.ControllerName, "Event");
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }
  
        public void DeleteConfirmedHasActionNameAttributeWithCorrectActionName()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Name, "Delete");
        }

        [Fact]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task AssignSendsTaskQueryAsyncWithCorrectTaskId()
        {
            const int taskId = 1;
            var taskModelSummary = new TaskSummaryModel { EventId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = 1 } });
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskModelSummary);

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();
            await sut.Assign(taskId, null);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQueryAsync>(y => y.TaskId == taskId)), Times.Once);
        }

        [Fact]
        public async Task AssignReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var taskModelSummary = new TaskSummaryModel { EventId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskModelSummary);
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = 1 } });

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();
            var result = await sut.Assign(1, null);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task AssignSendsAssignTaskCommandAsync()
        {
            const int organizationId = 1;
            const int taskId = 1;
            var taskModelSummary = new TaskSummaryModel { EventId = 1 };
            var userIds = new List<string> { "1", "2" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskModelSummary);
            mediator.Setup(x => x.Send(It.Is<EventByIdQuery>(y => y.EventId == taskModelSummary.EventId))).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = organizationId }});

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());
            await sut.Assign(taskId, userIds);

            mediator.Verify(x => x.SendAsync(It.Is<AssignTaskCommandAsync>(y => y.TaskId == taskId && y.UserIds == userIds)), Times.Once);
        }

        [Fact]
        public async Task AssignRedirectsToRoute()
        {
            const int organizationId = 1;
            const int taskId = 1;
            var taskModelSummary = new TaskSummaryModel { EventId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(taskModelSummary);
            mediator.Setup(x => x.Send(It.Is<EventByIdQuery>(y => y.EventId == taskModelSummary.EventId))).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = organizationId } });

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());

            var result = await sut.Assign(taskId, null) as RedirectToRouteResult;

            Assert.Equal(result.RouteValues["controller"], "Task");
            Assert.Equal(result.RouteValues["Area"], "Admin");
            Assert.Equal(result.RouteValues["action"], nameof(TaskController.Details));
            Assert.Equal(result.RouteValues["id"], taskId);
        }

        [Fact]
        public void AssignHasHttpPostAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Assign(It.IsAny<int>(), It.IsAny<List<string>>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void AssignHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.Assign(It.IsAny<int>(), It.IsAny<List<string>>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsBadRequestObjectResultWhenModelStateIsInvalid()
        {
            var sut = new TaskController(null);
            sut.AddModelStateError();
            var result = await sut.MessageAllVolunteers(It.IsAny<MessageTaskVolunteersModel>());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task MessageAllVounteersSendsTaskQueryAsyncWithCorrectTaskId()
        {
            var model = new MessageTaskVolunteersModel { TaskId = 1 };
            var mediator = new Mock<IMediator>();

            var sut = new TaskController(mediator.Object);
            await sut.MessageAllVolunteers(model);

            mediator.Verify(x => x.SendAsync(It.Is<TaskQueryAsync>(y => y.TaskId == model.TaskId)), Times.Once);
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var mediator = new Mock<IMediator>();

            var sut = new TaskController(mediator.Object);
            var result = await sut.MessageAllVolunteers(new MessageTaskVolunteersModel());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task MessageAllVolunttersReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(new TaskSummaryModel());

            var sut = new TaskController(mediator.Object);
            sut.SetDefaultHttpContext();
            var result = await sut.MessageAllVolunteers(new MessageTaskVolunteersModel());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task MessageAllVolunteersSendsMessageTaskVolunteersCommandAsyncWithCorrectData()
        {
            const int organizationId = 1;
            var model = new MessageTaskVolunteersModel();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(new TaskSummaryModel { OrganizationId = organizationId });

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());
            await sut.MessageAllVolunteers(model);

            mediator.Verify(x => x.SendAsync(It.Is<MessageTaskVolunteersCommandAsync>(y => y.Model ==  model)), Times.Once);
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsHttpOkResult()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskQueryAsync>())).ReturnsAsync(new TaskSummaryModel { OrganizationId = organizationId });

            var sut = new TaskController(mediator.Object);
            MakeUserOrganizationAdminUser(sut, organizationId.ToString());
            var result = await sut.MessageAllVolunteers(new MessageTaskVolunteersModel());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void MessageAllVolunteersHasHttpPostAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageTaskVolunteersModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void MessageAllVolunteersHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageTaskVolunteersModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new TaskController(null);
            sut.SetDefaultHttpContext();
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAuthorizeAtttributeWithTheCorrectPolicy()
        {
            var sut = new TaskController(null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        private static void MakeUserOrganizationAdminUser(Controller controller, string organizationId)
        {
            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId)
            };

            var httpContext = new Mock<HttpContext>();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(orgAdminClaims));
            httpContext.Setup(x => x.User).Returns(claimsPrincipal);

            controller.ControllerContext.HttpContext = httpContext.Object;
        }
    }
}