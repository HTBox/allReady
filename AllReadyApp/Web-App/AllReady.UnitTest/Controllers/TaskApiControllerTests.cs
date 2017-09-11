using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Controllers;
using AllReady.Features.Events;
using AllReady.Features.Tasks;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Shared;
using AllReady.ViewModels.Task;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using DeleteVolunteerTaskCommand = AllReady.Features.Tasks.DeleteVolunteerTaskCommand;

namespace AllReady.UnitTest.Controllers
{
    public class TaskApiControllerTests
    {
        #region Post
        [Fact]
        public async Task PostReturnsHttpUnauthorizedWhenUserDoesNotHaveTheAuthorizationToEditTheTaskOrTheTaskIsNotInAnEditableState()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(false);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Post(new VolunteerTaskViewModel { EventId = 1 });

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task PostReturnsBadRequestResultWhenTaskAlreadyExists()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>())).ReturnsAsync(new VolunteerTask());

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Post(new VolunteerTaskViewModel { EventId = 1 });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostReturnsBadRequestObjectResultWithCorrectErrorMessageWhenEventIsNull()
        {
            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(Mock.Of<IMediator>(), determineIfATaskIsEditable.Object, null);
            var result = await sut.Post(new VolunteerTaskViewModel()) as BadRequestObjectResult;

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task PostSendsAddTaskCommandAsyncWithCorrectData()
        {
            var model = new VolunteerTaskViewModel { EventId = 1, Id = 1 };
            var volunteerTask = new VolunteerTask();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());
            mediator.SetupSequence(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>()))
                .ReturnsAsync(volunteerTask).ReturnsAsync(null);

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            await sut.Post(model);

            mediator.Verify(x => x.SendAsync(It.Is<AddVolunteerTaskCommand>(y => y.VolunteerTask == volunteerTask)), Times.Once);
        }

        [Fact]
        public async Task PostSendsTaskByTaskIdQueryWithCorrectTaskId()
        {
            var model = new VolunteerTaskViewModel { EventId = 1, Id = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>())).ReturnsAsync(new VolunteerTask());

            var provider = new Mock<IDetermineIfATaskIsEditable>();
            provider.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, provider.Object, null);
            await sut.Post(model);

            mediator.Verify(x => x.SendAsync(It.Is<VolunteerTaskByVolunteerTaskIdQuery>(y => y.VolunteerTaskId == model.Id)));
        }

        [Fact]
        public async Task PostReturnsHttpStatusCodeResultOf201()
        {
            var model = new VolunteerTaskViewModel { EventId = 1, Id = 0 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());

            var provider = new Mock<IDetermineIfATaskIsEditable>();
            provider.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, provider.Object, null);
            var result = await sut.Post(model) as CreatedResult;

            Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public void PostHasHttpPostAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Post(It.IsAny<VolunteerTaskViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void PostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Post(It.IsAny<VolunteerTaskViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }
        #endregion

        #region Put
        [Fact]
        public async Task PutSendsTaskByTaskIdQueryWithCorrectTaskId()
        {
            const int volunteerTaskId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.Put(volunteerTaskId, It.IsAny<VolunteerTaskViewModel>());

            mediator.Verify(x => x.SendAsync(It.Is<VolunteerTaskByVolunteerTaskIdQuery>(y => y.VolunteerTaskId == volunteerTaskId)));
        }

        [Fact]
        public async Task PutReturnsBadRequestResultWhenCannotFindTaskByTaskId()
        {
            var sut = new TaskApiController(Mock.Of<IMediator>(), null, null);
            var result = await sut.Put(It.IsAny<int>(), It.IsAny<VolunteerTaskViewModel>());

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutReturnsHttpUnauthorizedResultWhenAUserDoesNotHavePermissionToEditTheTaskOrTheTaskIsNotEditable()
        {
            const int volunteerTaskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>())).ReturnsAsync(new VolunteerTask());

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(false);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Put(volunteerTaskId, It.IsAny<VolunteerTaskViewModel>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task PutSendsUpdateTaskCommandWithCorrectAllReadyTask()
        {
            var volunteerTask = new VolunteerTask();
            var model = new VolunteerTaskViewModel { Name = "name", Description = "description", StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>())).ReturnsAsync(volunteerTask);

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            await sut.Put(It.IsAny<int>(), model);

            mediator.Verify(x => x.SendAsync(It.Is<UpdateVolunteerTaskCommand>(y => y.VolunteerTask == volunteerTask)), Times.Once);
        }

        [Fact]
        public async Task PutReturnsHttpStatusCodeResultOf204()
        {
            var volunteerTask = new VolunteerTask();
            var model = new VolunteerTaskViewModel { Name = "name", Description = "description", StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>())).ReturnsAsync(volunteerTask);

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Put(It.IsAny<int>(), model) as NoContentResult;

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public void PutHasHttpPutAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Put(It.IsAny<int>(), It.IsAny<VolunteerTaskViewModel>())).OfType<HttpPutAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("{id}", attribute.Template);
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteSendsTaskByTaskIdQueryWithCorrectTaskId()
        {
            const int volunteerTaskId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.Delete(volunteerTaskId);

            mediator.Verify(x => x.SendAsync(It.Is<VolunteerTaskByVolunteerTaskIdQuery>(y => y.VolunteerTaskId == volunteerTaskId)));
        }

        [Fact]
        public async Task DeleteReturnsBadRequestResultWhenCannotFindTaskByTaskId()
        {
            var sut = new TaskApiController(Mock.Of<IMediator>(), null, null);
            var result = await sut.Delete(It.IsAny<int>());

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenAUserDoesNotHavePermissionToEditTheTaskOrTheTaskIsNotEditable()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>())).ReturnsAsync(new VolunteerTask());

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(false);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Delete(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteSendsDeleteTaskCommandWithCorrectTaskId()
        {
            var volunteerTask = new VolunteerTask { Id = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskByVolunteerTaskIdQuery>())).ReturnsAsync(volunteerTask);

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<VolunteerTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            await sut.Delete(It.IsAny<int>());

            mediator.Verify(x => x.SendAsync(It.Is<DeleteVolunteerTaskCommand>(y => y.VolunteerTaskId == volunteerTask.Id)));
        }

        [Fact]
        public void DeleteHasHttpDeleteAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Delete(It.IsAny<int>())).OfType<HttpDeleteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("{id}", attribute.Template);
        }
        #endregion

        #region RegisterTask
        [Fact]
        public async Task RegisterTaskReturnsHttpBadRequestWhenModelIsNull()
        {
            var sut = new TaskApiController(null, null, null);
            var result = await sut.RegisterTask(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RegisterTaskReturnsJsonWhenThereIsModelStateError()
        {
            const string modelStateErrorMessage = "modelStateErrorMessage";

            var sut = new TaskApiController(null, null, null);
            sut.AddModelStateErrorWithErrorMessage(modelStateErrorMessage);

            var jsonResult = await sut.RegisterTask(new VolunteerTaskSignupViewModel()) as JsonResult;
            var result = jsonResult.GetValueForProperty<List<string>>("errors");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<List<string>>(result);
            Assert.Equal(result.First(), modelStateErrorMessage);
        }

        [Fact]
        public async Task RegisterTaskSendsTaskSignupCommandWithCorrectTaskSignupModel()
        {
            var model = new VolunteerTaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<VolunteerTaskSignupCommand>(y => y.TaskSignupModel == model))).ReturnsAsync(new VolunteerTaskSignupResult());

            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.RegisterTask(model);

            mediator.Verify(x => x.SendAsync(It.Is<VolunteerTaskSignupCommand>(command => command.TaskSignupModel.Equals(model))));
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenApiResult_IsSuccess()
        {
            const TaskResultStatus volunteerTaskSignUpResultStatus = TaskResultStatus.Success;
            var model = new VolunteerTaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<VolunteerTaskSignupCommand>(y => y.TaskSignupModel == model)))
                .ReturnsAsync(new VolunteerTaskSignupResult
                {
                    Status = volunteerTaskSignUpResultStatus,
                    VolunteerTask = new VolunteerTask { Id = 1, Name = "Task" }
                });

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var volunteerTaskModel = jsonResult.GetValueForProperty<VolunteerTaskViewModel>("task");

            Assert.True(successStatus);
            Assert.NotNull(volunteerTaskModel);
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenEventNotFound()
        {
            const TaskResultStatus volunteerTaskSignUpResultStatus = TaskResultStatus.Failure_EventNotFound;

            var model = new VolunteerTaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<VolunteerTaskSignupCommand>(y => y.TaskSignupModel == model)))
                .ReturnsAsync(new VolunteerTaskSignupResult
                {
                    Status = volunteerTaskSignUpResultStatus                    
                });

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var errors = jsonResult.GetValueForProperty<string[]>("errors");

            Assert.False(successStatus);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("Signup failed - The event could not be found", errors[0]);
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenTaskNotFound()
        {
            const TaskResultStatus volunteerTaskSignUpResultStatus = TaskResultStatus.Failure_TaskNotFound;

            var model = new VolunteerTaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<VolunteerTaskSignupCommand>(y => y.TaskSignupModel == model)))
                .ReturnsAsync(new VolunteerTaskSignupResult
                {
                    Status = volunteerTaskSignUpResultStatus
                });

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var errors = jsonResult.GetValueForProperty<string[]>("errors");

            Assert.False(successStatus);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("Signup failed - The task could not be found", errors[0]);
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenTaskIsClosed()
        {
            const TaskResultStatus volunteerTaskSignUpResultStatus = TaskResultStatus.Failure_ClosedTask;

            var model = new VolunteerTaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<VolunteerTaskSignupCommand>(y => y.TaskSignupModel == model)))
                .ReturnsAsync(new VolunteerTaskSignupResult
                {
                    Status = volunteerTaskSignUpResultStatus
                });

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var errors = jsonResult.GetValueForProperty<string[]>("errors");

            Assert.False(successStatus);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("Signup failed - Task is closed", errors[0]);
        }

        [Fact]
        public void RegisterTaskHasValidateAntiForgeryTokenAttrbiute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<VolunteerTaskSignupViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterTaskHasHttpPostAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<VolunteerTaskSignupViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal("signup", attribute.Template);
        }

        [Fact]
        public void RegisterTaskHasAuthorizeAttrbiute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<VolunteerTaskSignupViewModel>())).OfType<AuthorizeAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterTaskHasHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<VolunteerTaskSignupViewModel>())).OfType<ProducesAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }
        #endregion

        #region UnregisterTask
        [Fact]
        public async Task UnregisterTaskSendsTaskUnenrollCommandWithCorrectTaskIdAndUserId()
        {
            const string userId = "1";
            const int volunteerTaskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskUnenrollCommand>())).ReturnsAsync(new VolunteerTaskUnenrollResult());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var sut = new TaskApiController(mediator.Object, null, userManager.Object);
            sut.SetFakeUser(userId);

            await sut.UnregisterTask(volunteerTaskId);

            mediator.Verify(x => x.SendAsync(It.Is<VolunteerTaskUnenrollCommand>(y => y.VolunteerTaskId == volunteerTaskId && y.UserId == userId)));
        }

        [Fact]
        public async Task UnregisterTaskReturnsCorrectStatus()
        {
            const string status = "status";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskUnenrollCommand>())).ReturnsAsync(new VolunteerTaskUnenrollResult { Status = status });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(It.IsAny<string>());

            var sut = new TaskApiController(mediator.Object, null, userManager.Object);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.UnregisterTask(It.IsAny<int>());
            var result = jsonResult.GetValueForProperty<string>("Status");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<string>(result);
            Assert.Equal(result, status);
        }

        [Fact]
        public async Task UnregisterTaskReturnsNullForTaskWhenResultTaskIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskUnenrollCommand>())).ReturnsAsync(new VolunteerTaskUnenrollResult());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(It.IsAny<string>());

            var sut = new TaskApiController(mediator.Object, null, userManager.Object);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.UnregisterTask(It.IsAny<int>());
            var result = jsonResult.GetValueForProperty<string>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.Null(result);
        }

        [Fact]
        public async Task UnregisterTaskReturnsTaskViewModelWhenResultTaskIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskUnenrollCommand>())).ReturnsAsync(new VolunteerTaskUnenrollResult { VolunteerTask = new VolunteerTask() });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(It.IsAny<string>());

            var sut = new TaskApiController(mediator.Object, null, userManager.Object);
            sut.SetDefaultHttpContext();
            
            var jsonResult = await sut.UnregisterTask(It.IsAny<int>());
            var result = jsonResult.GetValueForProperty<VolunteerTaskViewModel>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<VolunteerTaskViewModel>(result);
        }

        [Fact]
        public void UnregisterTaskHasAuthorizeAttrbiute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.UnregisterTask(It.IsAny<int>())).OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void UnregisterTaskHasHttpDeleteAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.UnregisterTask(It.IsAny<int>())).OfType<HttpDeleteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("{id}/signup", attribute.Template);
        }
        #endregion

        #region ChangeStatus
        [Fact]
        public async Task ChangeStatusInvokesSendAsyncWithCorrectTaskStatusChangeCommand()
        {
            var model = new VolunteerTaskChangeModel { VolunteerTaskId = 1, UserId = "1", Status = VolunteerTaskStatus.Accepted, StatusDescription = "statusDescription" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ChangeVolunteerTaskStatusCommand>())).ReturnsAsync(new VolunteerTaskChangeResult());

            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.ChangeStatus(model);

            mediator.Verify(x => x.SendAsync(It.Is<ChangeVolunteerTaskStatusCommand>(y => y.VolunteerTaskId == model.VolunteerTaskId && 
                y.VolunteerTaskStatus == model.Status && 
                y.VolunteerTaskStatusDescription == model.StatusDescription && 
                y.UserId == model.UserId)));
        }

        [Fact]
        public async Task ChangeStatusReturnsCorrectStatus()
        {
            const string status = "status";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ChangeVolunteerTaskStatusCommand>())).ReturnsAsync(new VolunteerTaskChangeResult { Status = status });

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.ChangeStatus(new VolunteerTaskChangeModel());
            var result = jsonResult.GetValueForProperty<string>("Status");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<string>(result);
            Assert.Equal(result, status);
        }

        [Fact]
        public async Task ChangeStatusReturnsNullForTaskWhenResultTaskIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ChangeVolunteerTaskStatusCommand>())).ReturnsAsync(new VolunteerTaskChangeResult { Status = "status" });

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.ChangeStatus(new VolunteerTaskChangeModel());
            var result = jsonResult.GetValueForProperty<string>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.Null(result);
        }

        [Fact]
        public async Task ChangeStatusReturnsTaskViewModelWhenResultTaskIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ChangeVolunteerTaskStatusCommand>())).ReturnsAsync(new VolunteerTaskChangeResult { VolunteerTask = new VolunteerTask() });

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.ChangeStatus(new VolunteerTaskChangeModel());
            var result = jsonResult.GetValueForProperty<VolunteerTaskViewModel>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<VolunteerTaskViewModel>(result);
        }

        [Fact]
        public void ChangeStatusHasHttpPostAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<VolunteerTaskChangeModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ChangeStatusHasAuthorizeAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<VolunteerTaskChangeModel>())).OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ChangeStatusHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<VolunteerTaskChangeModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ChangeStatusHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<VolunteerTaskChangeModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("changestatus", attribute.Template);
        }
        #endregion

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("api/task", attribute.Template);
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }
    }
}
