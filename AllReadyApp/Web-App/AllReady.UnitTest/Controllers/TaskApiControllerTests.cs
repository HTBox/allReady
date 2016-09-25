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
using DeleteTaskCommand = AllReady.Features.Tasks.DeleteTaskCommand;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

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
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(false);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Post(new TaskViewModel { EventId = 1 });

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task PostReturnsBadRequestResultWhenTaskAlreadyExists()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskByTaskIdQuery>())).ReturnsAsync(new AllReadyTask());

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Post(new TaskViewModel { EventId = 1 });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostReturnsBadRequestObjectResultWithCorrectErrorMessageWhenEventIsNull()
        {
            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

            var sut = new TaskApiController(Mock.Of<IMediator>(), determineIfATaskIsEditable.Object, null);
            var result = await sut.Post(new TaskViewModel()) as BadRequestObjectResult;

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(result.StatusCode, 400);
        }

        //TODO: mgmccarthy. commented out until I can figure out how to do SetupSequence on async invocations of mediator's SendAsync
        //[Fact]
        //public async Task PostSendsAddTaskCommandAsyncWithCorrectData()
        //{
        //    var model = new TaskViewModel { EventId = 1, Id = 1 };
        //    var allReadyTask = new AllReadyTask();

        //    var mediator = new Mock<IMediator>();
        //    mediator.Setup(x => x.Send(It.IsAny<EventByEventIdQueryAsync>())).Returns(new Event());
        //    mediator.SetupSequence(x => x.Send(It.IsAny<TaskByTaskIdQueryAsync>()))
        //        .Returns(allReadyTask)
        //        .Returns(null);

        //    var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
        //    determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

        //    var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
        //    await sut.Post(model);

        //    mediator.Verify(x => x.SendAsync(It.Is<AddTaskCommandAsync>(y => y.AllReadyTask == allReadyTask)), Times.Once);
        //}

        [Fact]
        public async Task PostSendsTaskByTaskIdQueryWithCorrectTaskId()
        {
            var model = new TaskViewModel { EventId = 1, Id = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskByTaskIdQuery>())).ReturnsAsync(new AllReadyTask());

            var provider = new Mock<IDetermineIfATaskIsEditable>();
            provider.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, provider.Object, null);
            await sut.Post(model);

            mediator.Verify(x => x.SendAsync(It.Is<TaskByTaskIdQuery>(y => y.TaskId == model.Id)));
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task PostReturnsHttpStatusCodeResultOf201()
        {
            var model = new TaskViewModel { EventId = 1, Id = 0 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event());

            var provider = new Mock<IDetermineIfATaskIsEditable>();
            provider.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, provider.Object, null);
            var result = await sut.Post(model) as StatusCodeResult;

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(result.StatusCode, 201);
        }

        [Fact]
        public void PostHasHttpPostAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Post(It.IsAny<TaskViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void PostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Post(It.IsAny<TaskViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }
        #endregion

        #region Put
        [Fact]
        public async Task PutSendsTaskByTaskIdQueryWithCorrectTaskId()
        {
            const int taskId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.Put(taskId, It.IsAny<TaskViewModel>());

            mediator.Verify(x => x.SendAsync(It.Is<TaskByTaskIdQuery>(y => y.TaskId == taskId)));
        }

        [Fact]
        public async Task PutReturnsBadRequestResultWhenCannotFindTaskByTaskId()
        {
            var sut = new TaskApiController(Mock.Of<IMediator>(), null, null);
            var result = await sut.Put(It.IsAny<int>(), It.IsAny<TaskViewModel>());

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutReturnsHttpUnauthorizedResultWhenAUserDoesNotHavePermissionToEditTheTaskOrTheTaskIsNotEditable()
        {
            const int taskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskByTaskIdQuery>())).ReturnsAsync(new AllReadyTask());

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(false);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Put(taskId, It.IsAny<TaskViewModel>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task PutSendsUpdateTaskCommandAsyncWithCorrectAllReadyTask()
        {
            var allReadyTask = new AllReadyTask();
            var model = new TaskViewModel { Name = "name", Description = "description", StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskByTaskIdQuery>())).ReturnsAsync(allReadyTask);

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            await sut.Put(It.IsAny<int>(), model);

            mediator.Verify(x => x.SendAsync(It.Is<UpdateTaskCommand>(y => y.AllReadyTask == allReadyTask)), Times.Once);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task PutReturnsHttpStatusCodeResultOf204()
        {
            var allReadyTask = new AllReadyTask();
            var model = new TaskViewModel { Name = "name", Description = "description", StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskByTaskIdQuery>())).ReturnsAsync(allReadyTask);

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Put(It.IsAny<int>(), model) as StatusCodeResult;

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(result.StatusCode, 204);
        }

        [Fact]
        public void PutHasHttpPutAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Put(It.IsAny<int>(), It.IsAny<TaskViewModel>())).OfType<HttpPutAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}");
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteSendsTaskByTaskIdQueryWithCorrectTaskId()
        {
            const int taskId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.Delete(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskByTaskIdQuery>(y => y.TaskId == taskId)));
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
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskByTaskIdQuery>())).ReturnsAsync(new AllReadyTask());

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(false);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            var result = await sut.Delete(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteSendsDeleteTaskCommandAsyncWithCorrectTaskId()
        {
            var allReadyTask = new AllReadyTask { Id = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskByTaskIdQuery>())).ReturnsAsync(allReadyTask);

            var determineIfATaskIsEditable = new Mock<IDetermineIfATaskIsEditable>();
            determineIfATaskIsEditable.Setup(x => x.For(It.IsAny<ClaimsPrincipal>(), It.IsAny<AllReadyTask>(), null)).Returns(true);

            var sut = new TaskApiController(mediator.Object, determineIfATaskIsEditable.Object, null);
            await sut.Delete(It.IsAny<int>());

            mediator.Verify(x => x.SendAsync(It.Is<DeleteTaskCommand>(y => y.TaskId == allReadyTask.Id)));
        }

        [Fact]
        public void DeleteHasHttpDeleteAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Delete(It.IsAny<int>())).OfType<HttpDeleteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}");
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

            var jsonResult = await sut.RegisterTask(new TaskSignupViewModel()) as JsonResult;
            var result = jsonResult.GetValueForProperty<List<string>>("errors");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<List<string>>(result);
            Assert.Equal(result.First(), modelStateErrorMessage);
        }

        [Fact]
        public async Task RegisterTaskSendsTaskSignupCommandWithCorrectTaskSignupModel()
        {
            var model = new TaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model))).Returns(Task.FromResult(new TaskSignupResult()));

            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.RegisterTask(model);

            mediator.Verify(x => x.SendAsync(It.Is<TaskSignupCommand>(command => command.TaskSignupModel.Equals(model))));
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenApiResult_IsSuccess()
        {
            const string taskSignUpResultStatus = TaskSignupResult.SUCCESS;
            var model = new TaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model)))
                .Returns(Task.FromResult(new TaskSignupResult
                {
                    Status = taskSignUpResultStatus,
                    Task = new AllReadyTask { Id = 1, Name = "Task" }
                }));

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var taskModel = jsonResult.GetValueForProperty<TaskViewModel>("task");

            Assert.True(successStatus);
            Assert.NotNull(taskModel);
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenEventNotFound()
        {
            const string taskSignUpResultStatus = TaskSignupResult.FAILURE_EVENTNOTFOUND;

            var model = new TaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model)))
                .Returns(Task.FromResult(new TaskSignupResult
                {
                    Status = taskSignUpResultStatus                    
                }));

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var errors = jsonResult.GetValueForProperty<string[]>("errors");

            Assert.False(successStatus);
            Assert.NotNull(errors);
            Assert.Equal(1, errors.Count());
            Assert.Equal("Signup failed - The event could not be found", errors[0]);
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenTaskNotFound()
        {
            const string taskSignUpResultStatus = TaskSignupResult.FAILURE_TASKNOTFOUND;

            var model = new TaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model)))
                .Returns(Task.FromResult(new TaskSignupResult
                {
                    Status = taskSignUpResultStatus
                }));

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var errors = jsonResult.GetValueForProperty<string[]>("errors");

            Assert.False(successStatus);
            Assert.NotNull(errors);
            Assert.Equal(1, errors.Count());
            Assert.Equal("Signup failed - The task could not be found", errors[0]);
        }

        [Fact]
        public async Task Register_ReturnsCorrectJson_WhenTaskIsClosed()
        {
            const string taskSignUpResultStatus = TaskSignupResult.FAILURE_CLOSEDTASK;

            var model = new TaskSignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model)))
                .Returns(Task.FromResult(new TaskSignupResult
                {
                    Status = taskSignUpResultStatus
                }));

            var sut = new TaskApiController(mediator.Object, null, null);

            var jsonResult = await sut.RegisterTask(model) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var errors = jsonResult.GetValueForProperty<string[]>("errors");

            Assert.False(successStatus);
            Assert.NotNull(errors);
            Assert.Equal(1, errors.Count());
            Assert.Equal("Signup failed - Task is closed", errors[0]);
        }

        [Fact]
        public void RegisterTaskHasValidateAntiForgeryTokenAttrbiute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<TaskSignupViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterTaskHasHttpPostAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<TaskSignupViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "signup");
        }

        [Fact]
        public void RegisterTaskHasAuthorizeAttrbiute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<TaskSignupViewModel>())).OfType<AuthorizeAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterTaskHasHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<TaskSignupViewModel>())).OfType<ProducesAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x).First(), "application/json");
        }
        #endregion

        #region UnregisterTask
        [Fact(Skip = "RTM Broken Tests")]
        public async Task UnregisterTaskSendsTaskUnenrollCommandAsyncWithCorrectTaskIdAndUserId()
        {
            const string userId = "1";
            const int taskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskUnenrollCommand>())).ReturnsAsync(new TaskUnenrollResult());

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetFakeUser(userId);

            await sut.UnregisterTask(taskId);

            mediator.Verify(x => x.SendAsync(It.Is<TaskUnenrollCommand>(y => y.TaskId == taskId && y.UserId == userId)));
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UnregisterTaskReturnsCorrectStatus()
        {
            const string status = "status";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskUnenrollCommand>())).ReturnsAsync(new TaskUnenrollResult { Status = status });

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.UnregisterTask(It.IsAny<int>());
            var result = jsonResult.GetValueForProperty<string>("Status");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<string>(result);
            Assert.Equal(result, status);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UnregisterTaskReturnsNullForTaskWhenResultTaskIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskUnenrollCommand>())).ReturnsAsync(new TaskUnenrollResult());

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.UnregisterTask(It.IsAny<int>());
            var result = jsonResult.GetValueForProperty<string>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.Null(result);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UnregisterTaskReturnsTaskViewModelWhenResultTaskIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskUnenrollCommand>())).ReturnsAsync(new TaskUnenrollResult { Task = new AllReadyTask() });

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();
            
            var jsonResult = await sut.UnregisterTask(It.IsAny<int>());
            var result = jsonResult.GetValueForProperty<TaskViewModel>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<TaskViewModel>(result);
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
            Assert.Equal(attribute.Template, "{id}/signup");
        }
        #endregion

        #region ChangeStatus
        [Fact]
        public async Task ChangeStatusInvokesSendAsyncWithCorrectTaskStatusChangeCommand()
        {
            var model = new TaskChangeModel { TaskId = 1, UserId = "1", Status = TaskStatus.Accepted, StatusDescription = "statusDescription" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskStatusChangeCommandAsync>())).Returns(() => Task.FromResult(new TaskChangeResult()));

            var sut = new TaskApiController(mediator.Object, null, null);
            await sut.ChangeStatus(model);

            mediator.Verify(x => x.SendAsync(It.Is<TaskStatusChangeCommandAsync>(y => y.TaskId == model.TaskId && 
                y.TaskStatus == model.Status && 
                y.TaskStatusDescription == model.StatusDescription && 
                y.UserId == model.UserId)));
        }

        [Fact]
        public async Task ChangeStatusReturnsCorrectStatus()
        {
            const string status = "status";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskStatusChangeCommandAsync>())).Returns(() => Task.FromResult(new TaskChangeResult { Status = status }));

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.ChangeStatus(new TaskChangeModel());
            var result = jsonResult.GetValueForProperty<string>("Status");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<string>(result);
            Assert.Equal(result, status);
        }

        [Fact]
        public async Task ChangeStatusReturnsNullForTaskWhenResultTaskIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskStatusChangeCommandAsync>())).Returns(() => Task.FromResult(new TaskChangeResult { Status = "status" }));

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.ChangeStatus(new TaskChangeModel());
            var result = jsonResult.GetValueForProperty<string>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.Null(result);
        }

        [Fact]
        public async Task ChangeStatusReturnsTaskViewModelWhenResultTaskIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<TaskStatusChangeCommandAsync>())).Returns(() => Task.FromResult(new TaskChangeResult { Task = new AllReadyTask() }));

            var sut = new TaskApiController(mediator.Object, null, null);
            sut.SetDefaultHttpContext();

            var jsonResult = await sut.ChangeStatus(new TaskChangeModel());
            var result = jsonResult.GetValueForProperty<TaskViewModel>("Task");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<TaskViewModel>(result);
        }

        [Fact]
        public void ChangeStatusHasHttpPostAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<TaskChangeModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ChangeStatusHasAuthorizeAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<TaskChangeModel>())).OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ChangeStatusHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<TaskChangeModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ChangeStatusHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ChangeStatus(It.IsAny<TaskChangeModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "changestatus");
        }
        #endregion

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/task");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new TaskApiController(null, null, null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x).First(), "application/json");
        }
    }
}
