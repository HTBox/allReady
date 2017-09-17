using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.UnlinkedRequests;
using AllReady.Areas.Admin.ViewModels.UnlinkedRequests;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class UnlinkedRequestControllerTests
    {
        private const int OrganizationId = 1001;
        private const int EventId = 123;

        [Fact]
        public async void ListReturnsRequestUnathorized_WhenUserIsNotAnOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserNotAnOrgAdmin();

            await sut.List();
            Assert.IsType<UnauthorizedResult>(await sut.List());
        }

        [Fact]
        public async void ListCallsRequestListItemsQueryWithUsersOrgId_WhenUserIsOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());

            await sut.List();

            mediator.Verify(x => x.SendAsync(It.Is<UnlinkedRequestListQuery>(y => y.OrganizationId == 1001)), Times.Once);
        }

        [Fact]
        public async void AddRequestsReturnsUnauthorised_WhenUserIsNotAnOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            var model = new Mock<UnlinkedRequestViewModel>();
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.AddRequests(model.Object));
        }

        [Fact]
        public async void AddReqeustsCallsQueryHandlerWithExpectedOrgId_WhenModelHasValidationErrors()
        {
            var errorList = new List<KeyValuePair<string, string>>() {new KeyValuePair<string, string>("test", "error")};
            var model = new UnlinkedRequestViewModel()
            {
                EventId = EventId,
            };
            var mediator = BuildValidMockMediator(model);

            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(errorList);
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());
            await sut.AddRequests(model);

            mediator.Verify(x => x.SendAsync(It.Is<UnlinkedRequestListQuery>(y => y.OrganizationId == 1001)), Times.Once);
        }

        [Fact]
        public async void AddReqeustsReturnsExpectedResultType_WhenModelHasValidationErrors()
        {
            var errorList = new List<KeyValuePair<string, string>>() {new KeyValuePair<string, string>("test", "error")};
            var model = BuildValidModel();
            var mediator = BuildValidMockMediator(model);

            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(errorList);
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());
           
            var result = await sut.AddRequests(model);
            Assert.IsType<ViewResult>(result);
            Assert.IsType<UnlinkedRequestViewModel>(((ViewResult) result).ViewData.Model);
            Assert.Equal(nameof(sut.List), ((ViewResult) result).ViewName);
        }

        [Fact]
        public async void AddReqeustsReturnsExpectedViewName_WhenModelHasValidationErrors()
        {
            const string orgId = "1001";
            var errorList = new List<KeyValuePair<string, string>>() {new KeyValuePair<string, string>("test", "error")};
            var model = BuildValidModel();
            var mediator = BuildValidMockMediator(model);
            
            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(errorList);
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(orgId);

            var result = await sut.AddRequests(model);
            Assert.Equal(nameof(sut.List), ((ViewResult) result).ViewName);
        }

        [Fact]
        public async void AddReqeustsReturnModelContainsExpectedErrors_WhenModelHasValidationErrors()
        {
            var errorList = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("key1", "error1"),
                new KeyValuePair<string, string>("key2", "error2")
            };
            var model = BuildValidModel();
            var mediator = BuildValidMockMediator(model);
            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(errorList);
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());

            await sut.AddRequests(model);

            Assert.Equal(sut.ModelState.Count, errorList.Count);
            Assert.Equal(sut.ModelState.Keys, errorList.Select(error => error.Key));
            Assert.Equal(
                sut.ModelState.Select(
                    error => error.Value.Errors.Select(message => message.ErrorMessage).FirstOrDefault()),
                errorList.Select(error => error.Value));
        }

        [Fact]
        public async void AddReqeustsReturnModelContainsExpectedData_WhenModelHasValidationErrors()
        {
            var model = new UnlinkedRequestViewModel { EventId = EventId };
            var mediator = BuildValidMockMediator(model);
            var expectedEvents = new List<SelectListItem>()
            {
                new SelectListItem {Text = "testItem", Value = "testValue", Selected = true}
            };
            var expectedRequests = new List<RequestSelectViewModel>()
            {
                new RequestSelectViewModel {Name = "testRequest", Id = Guid.NewGuid()}
            };
            mediator.Setup(x => x.SendAsync(It.Is<UnlinkedRequestListQuery>(y => y.OrganizationId == 1001)))
                .ReturnsAsync(new UnlinkedRequestViewModel()
                {
                    Events = expectedEvents,
                    Requests = expectedRequests
                });

            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(new List<KeyValuePair<string, string>>() {new KeyValuePair<string, string>("test", "error")});
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());
          

            var result = (ViewResult) await sut.AddRequests(model);
            var returnedModel = (UnlinkedRequestViewModel) result.Model;
            Assert.Equal(returnedModel.Requests, expectedRequests);
            Assert.Equal(returnedModel.Events, expectedEvents);
            Assert.Equal(returnedModel.EventId, EventId);
        }

        [Fact]
        public async void AddReqeustsCallsCommandHandler_WhenModelStateIsValid()
        {
            var model = BuildValidModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<EventSummaryQuery>(y => y.EventId == EventId)))
                .ReturnsAsync(new EventSummaryViewModel()
                {
                    OrganizationId = 1001
                });

            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(new List<KeyValuePair<string, string>>());
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());
            await sut.AddRequests(model);

            mediator.Verify(x => x.SendAsync(It.Is<AddRequestsToEventCommand>(y => y.EventId == EventId)), Times.Once);
        }

        [Fact]
        public async void AddReqeustsReturnsRedirectResult_WhenModelStateIsValid()
        {
            var model = BuildValidModel();
            var mediator = BuildValidMockMediator(model);
            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(new List<KeyValuePair<string, string>>());
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());

            var result = await sut.AddRequests(model);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(sut.List), ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async void AddReqeustsReturnsUnauthorizedResult_WhenEventDoesNotBelongToCurrentOrgId()
        {
            var model = BuildValidModel();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<UnlinkedRequestListQuery>(y => y.OrganizationId == OrganizationId)))
                .ReturnsAsync(model);
            mediator.Setup(x => x.SendAsync(It.Is<EventSummaryQuery>(y => y.EventId == EventId)))
                .ReturnsAsync(new EventSummaryViewModel()
                {
                    OrganizationId = 6783
                });
            var validator = new Mock<IUnlinkedRequestViewModelValidator>();
            validator.Setup(mock => mock.Validate(It.IsAny<UnlinkedRequestViewModel>()))
                .Returns(new List<KeyValuePair<string, string>>());
            var sut = new UnlinkedRequestController(mediator.Object, validator.Object);
            sut.MakeUserAnOrgAdmin(OrganizationId.ToString());

            var result = await sut.AddRequests(model);
            Assert.IsType<UnauthorizedResult>(result);
        }
        

        private UnlinkedRequestViewModel BuildValidModel()
        {
            return new UnlinkedRequestViewModel()
            {
                EventId = EventId,
                Requests = new List<RequestSelectViewModel>
                {
                    new RequestSelectViewModel {Name = "testRequest", Id = Guid.NewGuid(), IsSelected = true},
                    new RequestSelectViewModel {Name = "testRequest2", Id = Guid.NewGuid(), IsSelected = false},
                }
            };
        }

        private Mock<IMediator> BuildValidMockMediator(UnlinkedRequestViewModel model)
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<UnlinkedRequestListQuery>(y => y.OrganizationId == OrganizationId)))
                .ReturnsAsync(model);
            mediator.Setup(x => x.SendAsync(It.Is<EventSummaryQuery>(y => y.EventId == EventId)))
                .ReturnsAsync(new EventSummaryViewModel()
                {
                    OrganizationId = OrganizationId
                });
            return mediator;
        }
    }
}