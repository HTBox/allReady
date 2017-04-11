using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Areas.Admin.ViewModels.Validators.Task;
using AllReady.Extensions;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class TaskControllerTests
    {
        [Fact]
        public async Task CreateGetSendsCreateTaskQueryWithTheCorrectEventId()
        {
            const int eventId = 1;
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CreateVolunteerTaskQuery>())).ReturnsAsync(new EditViewModel { OrganizationId = organizationId });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            await sut.Create(eventId);
            mediator.Verify(x => x.SendAsync(It.Is<CreateVolunteerTaskQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public async Task CreateGetReturnsUnauthorizedResultWhenUserIsNotOrganizationAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CreateVolunteerTaskQuery>())).ReturnsAsync(new EditViewModel());

            var sut = new TaskController(mediator.Object, null);
            sut.SetDefaultHttpContext();

            var result = await sut.Create(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CreateGetInvokesUrlActionWithTheCorrectParameters()
        {
            var model = new EditViewModel { EventId = 1, OrganizationId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CreateVolunteerTaskQuery>())).ReturnsAsync(model);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(model.OrganizationId.ToString());

            await sut.Create(It.IsAny<int>());

            urlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(y =>
                y.Action == nameof(EventController.Details) &&
                y.Controller == "Event" &&
                y.Values.ToString() == $"{{ id = {model.EventId}, area = Admin }}")),
            Times.Once);
        }

        [Fact]
        public async Task CreateGetAssignsCancelUrlToModel()
        {
            const string cancelUrl = "url";
            var model = new EditViewModel { EventId = 1, OrganizationId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CreateVolunteerTaskQuery>())).ReturnsAsync(model);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(cancelUrl);

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(model.OrganizationId.ToString());

            var result = await sut.Create(It.IsAny<int>()) as ViewResult;
            var modelResult = result.Model as EditViewModel;

            Assert.Equal(modelResult.CancelUrl, cancelUrl);
        }

        [Fact]
        public async Task CreateGetSetCorrectToTitleOnViewBag()
        {
            var model = new EditViewModel { EventId = 1, OrganizationId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CreateVolunteerTaskQuery>())).ReturnsAsync(model);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(model.OrganizationId.ToString());

            await sut.Create(It.IsAny<int>());

            Assert.Equal(sut.ViewBag.Title, "Create Task");
        }

        [Fact]
        public async Task CreateGetReturnsCorrectViewModel()
        {
            const int organizationId = 1;
            var model = new EditViewModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CreateVolunteerTaskQuery>())).ReturnsAsync(model);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Create(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as EditViewModel;

            Assert.IsType<EditViewModel>(modelResult);
        }

        [Fact]
        public async Task CreateGetReturnsCorrectView()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CreateVolunteerTaskQuery>())).ReturnsAsync(new EditViewModel { OrganizationId = organizationId });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Create(It.IsAny<int>()) as ViewResult;

            Assert.Equal(result.ViewName, "Edit");
        }

        [Fact]
        public void CreateGetHasHttpGetAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateGetHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Task/Create/{eventId}");
        }

        [Fact]
        public async Task EditGetSendsEditTaskQueryWithCorrectTaskId()
        {
            const int volunteerTaskId = 1;
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskQuery>())).ReturnsAsync(new EditViewModel { OrganizationId = organizationId });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Edit(volunteerTaskId);

            mediator.Verify(x => x.SendAsync(It.Is<EditVolunteerTaskQuery>(y => y.VolunteerTaskId == volunteerTaskId)), Times.Once);
        }

        [Fact]
        public async Task EditGetReturnsUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskQuery>())).ReturnsAsync(new EditViewModel());

            var sut = new TaskController(mediator.Object, null);
            sut.SetDefaultHttpContext();
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditGetAssignsCancelUrlToModel()
        {
            const string cancelUrl = "url";
            var model = new EditViewModel { EventId = 1, OrganizationId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskQuery>())).ReturnsAsync(model);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(cancelUrl);

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(model.OrganizationId.ToString());

            var result = await sut.Edit(It.IsAny<int>()) as ViewResult;
            var modelResult = result.Model as EditViewModel;

            Assert.Equal(modelResult.CancelUrl, cancelUrl);
        }

        [Fact]
        public async Task EditGetPopulatesCorrectTitleOnViewBag()
        {
            var model = new EditViewModel { EventId = 1, OrganizationId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskQuery>())).ReturnsAsync(model);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(model.OrganizationId.ToString());

            await sut.Edit(It.IsAny<int>());

            Assert.Equal(sut.ViewBag.Title, "Edit Task");
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewModelAndView()
        {
            const int organizationId = 1;
            var editViewModel = new EditViewModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskQuery>())).ReturnsAsync(editViewModel);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(mediator.Object, null) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Edit(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as EditViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<EditViewModel>(modelResult);
            Assert.Equal(modelResult, editViewModel);
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

        [Fact]
        public async Task EditPostInvokesValidateOnTaskSummaryModelValidatorWithCorrectParameters()
        {
            var model = new EditViewModel { EndDateTime = DateTimeOffset.Now.AddDays(-1), StartDateTime = DateTimeOffset.Now.AddDays(1), EventId = 1 };

            var validator = new Mock<IValidateVolunteerTaskEditViewModelValidator>();
            validator.Setup(x => x.Validate(model)).ReturnsAsync(new List<KeyValuePair<string, string>>()).Verifiable();

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(null, validator.Object) { Url = urlHelper.Object };
            sut.AddModelStateError();

            await sut.Edit(model);

            validator.Verify();
        }

        [Fact]
        public async Task EditPostAddsCorrectModelStateErrorWhenValidatorReturnsError()
        {
            const string errorKey = "ErrorKey";
            const string errorValue = "ErrorValue";

            var validator = new Mock<IValidateVolunteerTaskEditViewModelValidator>();
            validator.Setup(x => x.Validate(It.IsAny<EditViewModel>()))
                .ReturnsAsync(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(errorKey, errorValue) });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(null, validator.Object) { Url = urlHelper.Object };
            await sut.Edit(new EditViewModel() { EventId = 1, Id = 1 });

            var modelStateErrorCollection = sut.ModelState.GetErrorMessagesByKey(errorKey);
            Assert.Equal(modelStateErrorCollection.Single().ErrorMessage, errorValue);
        }

        [Fact]
        public async Task EditPostReturnsCorrectViewAndViewModelWhenValidatorReturnsError()
        {
            var model = new EditViewModel();

            var validator = new Mock<IValidateVolunteerTaskEditViewModelValidator>();
            validator.Setup(x => x.Validate(It.IsAny<EditViewModel>()))
                .ReturnsAsync(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("key", "value") });
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new TaskController(Mock.Of<IMediator>(), validator.Object) { Url = urlHelper.Object };
            var result = await sut.Edit(model) as ViewResult;
            var modelResult = result.ViewData.Model as EditViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(modelResult, model);
        }

        [Fact]
        public async Task EditPostReturnsUnauthorizedResultWhenModelStateIsValidAndUserIsNotAnOrganizationAdminUser()
        {
            var validator = new Mock<IValidateVolunteerTaskEditViewModelValidator>();
            validator.Setup(x => x.Validate(It.IsAny<EditViewModel>())).ReturnsAsync(new List<KeyValuePair<string, string>>());

            var sut = new TaskController(Mock.Of<IMediator>(), validator.Object);
            sut.SetDefaultHttpContext();

            var result = await sut.Edit(new EditViewModel());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditPostSendsEditTaskCommandWithCorrectModel_WhenModelStateIsValidAndUserIsOrganizationAdmin()
        {
            const int organizationId = 1;

            var model = new EditViewModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskCommand>())).ReturnsAsync(1);

            var validator = new Mock<IValidateVolunteerTaskEditViewModelValidator>();
            validator.Setup(x => x.Validate(It.IsAny<EditViewModel>())).ReturnsAsync(new List<KeyValuePair<string, string>>());

            var sut = new TaskController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            await sut.Edit(model);

            mediator.Verify(x => x.SendAsync(It.Is<EditVolunteerTaskCommand>(y => y.VolunteerTask == model)));
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectAction_WhenCreatingTask()
        {
            const int organizationId = 1;

            var model = new EditViewModel { Id = 0, OrganizationId = organizationId, EventId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskCommand>())).ReturnsAsync(1);

            var validator = new Mock<IValidateVolunteerTaskEditViewModelValidator>();
            validator.Setup(x => x.Validate(It.IsAny<EditViewModel>())).ReturnsAsync(new List<KeyValuePair<string, string>>());

            var sut = new TaskController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Edit(model) as RedirectToActionResult;

            var routeValues = new Dictionary<string, object> { ["id"] = model.EventId };

            Assert.Equal(result.ControllerName, "Event");
            Assert.Equal(result.ActionName, nameof(EventController.Details));
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectAction_WhenUpdatingTask()
        {
            const int organizationId = 1;

            var model = new EditViewModel { Id = 1, OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditVolunteerTaskCommand>())).ReturnsAsync(1);

            var validator = new Mock<IValidateVolunteerTaskEditViewModelValidator>();
            validator.Setup(x => x.Validate(It.IsAny<EditViewModel>())).ReturnsAsync(new List<KeyValuePair<string, string>>());

            var sut = new TaskController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Edit(model) as RedirectToActionResult;

            var routeValues = new Dictionary<string, object> { ["id"] = model.Id };

            Assert.Equal(result.ControllerName, "Task");
            Assert.Equal(result.ActionName, nameof(TaskController.Details));
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void EditPostHasHttpPostAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EditViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EditViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task DeleteSendsDeleteQueryWithCorrectTaskId()
        {
            const int volunteerTaskId = 1;
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>())).ReturnsAsync(new DeleteViewModel { OrganizationId = organizationId });

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Delete(volunteerTaskId);

            mediator.Verify(x => x.SendAsync(It.Is<DeleteQuery>(y => y.VolunteerTaskId == volunteerTaskId)), Times.Once);
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>())).ReturnsAsync(new DeleteViewModel());

            var sut = new TaskController(mediator.Object, null);
            sut.SetDefaultHttpContext();
            var result = await sut.Delete(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewModelAndView()
        {
            const int organizationId = 1;
            var deleteViewModel = new DeleteViewModel { OrganizationId = organizationId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>())).ReturnsAsync(deleteViewModel);

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as DeleteViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<DeleteViewModel>(modelResult);
            Assert.Equal(modelResult, deleteViewModel);
        }

        [Fact]
        public async Task DetailsSendsDetailsQueryWithCorrectTaskId()
        {
            const int volunteerTaskId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new TaskController(mediator.Object, null);
            await sut.Details(volunteerTaskId);

            mediator.Verify(x => x.SendAsync(It.Is<DetailsQuery>(y => y.VolunteerTaskId == volunteerTaskId)), Times.Once);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenTaskIsNull()
        {
            var sut = new TaskController(Mock.Of<IMediator>(), null);
            var result = await sut.Details(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModelAndView()
        {
            var detailsViewModel = new DetailsViewModel();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DetailsQuery>())).ReturnsAsync(detailsViewModel);

            var sut = new TaskController(mediator.Object, null);
            var result = await sut.Details(It.IsAny<int>()) as ViewResult;
            var modelResult = result.ViewData.Model as DetailsViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<DetailsViewModel>(modelResult);
            Assert.Equal(modelResult, detailsViewModel);
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
        public async Task DeleteConfirmedReturnsUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var model = new DeleteViewModel { UserIsOrgAdmin = false };

            var sut = new TaskController(null, null);
            sut.SetDefaultHttpContext();

            var result = await sut.DeleteConfirmed(model);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmedSendsDeleteTaskCommandWithCorrectTaskId()
        {
            const int volunteerTaskId = 1;
            var mediator = new Mock<IMediator>();

            var sut = new TaskController(mediator.Object, null);

            await sut.DeleteConfirmed(new DeleteViewModel { Id = volunteerTaskId, UserIsOrgAdmin = true });

            mediator.Verify(x => x.SendAsync(It.Is<DeleteVolunteerTaskCommand>(y => y.VolunteerTaskId == volunteerTaskId)), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmedRedirectsToCorrectAction()
        {
            const int organizationId = 1;

            var deleteViewModel = new DeleteViewModel { OrganizationId = organizationId, EventId = 1, UserIsOrgAdmin = true };

            var mediator = new Mock<IMediator>();

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.DeleteConfirmed(deleteViewModel) as RedirectToActionResult;

            var routeValues = new Dictionary<string, object> { ["id"] = deleteViewModel.EventId };

            Assert.Equal(result.ActionName, nameof(EventController.Details));
            Assert.Equal(result.ControllerName, "Event");
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        public void DeleteConfirmedHasActionNameAttributeWithCorrectActionName()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Name, "Delete");
        }

        [Fact]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task AssignSendsOrganizationIdByTaskIdQueryWithCorrectTaskId()
        {
            const int volunteerTaskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(It.IsAny<int>());

            var sut = new TaskController(mediator.Object, null);
            sut.SetDefaultHttpContext();
            await sut.Assign(volunteerTaskId, null);

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationIdByVolunteerTaskIdQuery>(y => y.VolunteerTaskId == volunteerTaskId)), Times.Once);
        }

        [Fact]
        public async Task AssignReturnsUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(It.IsAny<int>());

            var sut = new TaskController(mediator.Object, null);
            sut.SetDefaultHttpContext();
            var result = await sut.Assign(1, null);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task AssignSendsAssignTaskCommand()
        {
            const int organizationId = 1;
            const int volunteerTaskId = 1;
            var userIds = new List<string> { "1", "2" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(organizationId);

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Assign(volunteerTaskId, userIds);

            mediator.Verify(x => x.SendAsync(It.Is<AssignVolunteerTaskCommand>(y => y.VolunteerTaskId == volunteerTaskId && y.UserIds == userIds)), Times.Once);
        }

        [Fact]
        public async Task AssignRedirectsToCorrectRoute()
        {
            const int organizationId = 1;
            const int volunteerTaskId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(organizationId);

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Assign(volunteerTaskId, null) as RedirectToRouteResult;

            Assert.Equal(result.RouteValues["controller"], "Task");
            Assert.Equal(result.RouteValues["Area"], "Admin");
            Assert.Equal(result.RouteValues["action"], nameof(TaskController.Details));
            Assert.Equal(result.RouteValues["id"], volunteerTaskId);
        }

        [Fact]
        public void AssignHasHttpPostAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Assign(It.IsAny<int>(), It.IsAny<List<string>>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void AssignHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Assign(It.IsAny<int>(), It.IsAny<List<string>>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsBadRequestObjectResultWhenModelStateIsInvalid()
        {
            var sut = new TaskController(null, null);
            sut.AddModelStateError();
            var result = await sut.MessageAllVolunteers(It.IsAny<MessageTaskVolunteersViewModel>());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task MessageAllVounteersSendsOrganizationIdByTaskIdQueryWithCorrectTaskId()
        {
            const int organizationId = 1;
            var model = new MessageTaskVolunteersViewModel { VolunteerTaskId = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(organizationId);

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.MessageAllVolunteers(model);

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationIdByVolunteerTaskIdQuery>(y => y.VolunteerTaskId == model.VolunteerTaskId)), Times.Once);
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(It.IsAny<int>());

            var sut = new TaskController(mediator.Object, null);
            sut.SetDefaultHttpContext();
            var result = await sut.MessageAllVolunteers(new MessageTaskVolunteersViewModel());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task MessageAllVolunteersSendsMessageTaskVolunteersCommandWithCorrectData()
        {
            const int organizationId = 1;
            var model = new MessageTaskVolunteersViewModel();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(organizationId);

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.MessageAllVolunteers(model);

            mediator.Verify(x => x.SendAsync(It.Is<MessageTaskVolunteersCommand>(y => y.Model == model)), Times.Once);
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsHttpOkResult()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdByVolunteerTaskIdQuery>())).ReturnsAsync(organizationId);

            var sut = new TaskController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            var result = await sut.MessageAllVolunteers(new MessageTaskVolunteersViewModel());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void MessageAllVolunteersHasHttpPostAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageTaskVolunteersViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void MessageAllVolunteersHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new TaskController(null, null);
            var attribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageTaskVolunteersViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new TaskController(null, null);
            sut.SetDefaultHttpContext();
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
    }
}