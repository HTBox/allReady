using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Features.Tasks;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class TaskApiControllerTests
    {
        //Post
        //Put
        //Delete

        //these do not use HasEditTaskPermissions
        //RegisterTask
        [Fact]
        public async Task RegisterTaskReturnsHttpBadRequestWhenModelIsNull()
        {
            var sut = new TaskApiController(null, null);
            var result = await sut.RegisterTask(null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RegisterTaskReturnsJsonWhenThereIsModelStateError()
        {
            const string modelStateErrorMessage = "modelStateErrorMessage";

            var sut = new TaskApiController(null, null);
            sut.AddModelStateError(modelStateErrorMessage);

            var jsonResult = (JsonResult)await sut.RegisterTask(new ActivitySignupViewModel());
            var result = jsonResult.GetValueForProperty<List<string>>("errors");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<List<string>>(result);
            Assert.Equal(result.First(), modelStateErrorMessage);
        }

        [Fact]
        public async Task RegisterTaskSendsTaskSignupCommandWithCorrectTaskSignupModel()
        {
            var model = new ActivitySignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model))).Returns(Task.FromResult(new TaskSignupResult()));

            var sut = new TaskApiController(null, mediator.Object);
            await sut.RegisterTask(model);

            mediator.Verify(x => x.SendAsync(It.Is<TaskSignupCommand>(command => command.TaskSignupModel.Equals(model))));
        }

        [Fact]
        public async Task RegisterTaskReturnsCorrectValueForStatus()
        {
            var model = new ActivitySignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model))).Returns(Task.FromResult(new TaskSignupResult { Status = "status" }));

            var sut = new TaskApiController(null, mediator.Object);
            var result = await sut.RegisterTask(model);

            Assert.Equal(result.ToString(), "{ Status = status, Task =  }");
        }

        [Fact]
        public async Task RegisterTaskReturnsNullForTaskIfTaskIsNull()
        {
            var model = new ActivitySignupViewModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model))).Returns(Task.FromResult(new TaskSignupResult()));

            var sut = new TaskApiController(null, mediator.Object);
            var result = await sut.RegisterTask(model);

            Assert.Equal(result.ToString(), "{ Status = , Task =  }");
        }

        //[Fact]
        //public async Task RegisterTaskReturnsTaskViewModelWithCorrectDataForTaskIfTaskIsNotNull()
        //{
        //    var model = new ActivitySignupViewModel { UserId = "userId" };
        //    var mediator = new Mock<IMediator>();
        //    mediator.Setup(x => x.SendAsync(It.Is<TaskSignupCommand>(y => y.TaskSignupModel == model))).Returns(Task.FromResult(new TaskSignupResult { Task = new AllReadyTask() }));

        //    var sut = new TaskApiController(null, mediator.Object);
        //    //this is null when ReigeterTask's return type is ""Task<object>"
        //    //var jsonResult = await sut.RegisterTask(model) as JsonResult;
        //    var result = await sut.RegisterTask(model);

        //    //TODO: is there a way to re-serialize an anonynous type in json format?
        //    Assert.Equal(result.ToString(), "{ Status = , Task = AllReady.ViewModels.TaskViewModel }");
        //}

        [Fact]
        public void RegisterTaskHasValidateAntiForgeryTokenAttrbiute()
        {
            var sut = new TaskApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<ActivitySignupViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterTaskHasHttpPostAttributeWithCorrectTemplate()
        {
            var sut = new TaskApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<ActivitySignupViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "signup");
        }

        [Fact]
        public void RegisterTaskHasAuthorizeAttrbiute()
        {
            var sut = new TaskApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<ActivitySignupViewModel>())).OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterTaskHasHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new TaskApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.RegisterTask(It.IsAny<ActivitySignupViewModel>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }

        //UnregisterTask
        //ChangeStatus

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new TaskApiController(null, null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/task");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new TaskApiController(null, null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }

        //method attributes
    }
}
