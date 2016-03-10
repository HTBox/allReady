using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.Features.Activity;
using AllReady.Features.Notifications;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNet.Authorization;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc;

namespace AllReady.UnitTest.Controllers
{
    public class ActivityApiControllerTests
    {
        [Fact]
        public void SendsGetActivitiesWithUnlockedCampaignsQuery()
        {
            var mediator = new Mock<IMediator>();
            var sut = new ActivityApiController(mediator.Object);
            sut.Get();

            mediator.Verify(x => x.Send(It.IsAny<ActivitiesWithUnlockedCampaignsQuery>()), Times.Once);
        }

        [Fact]
        public void GetReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivitiesWithUnlockedCampaignsQuery>())).Returns(new List<ActivityViewModel>());

            var sut = new ActivityApiController(mediator.Object);
            var results = sut.Get();

            Assert.IsType<List<ActivityViewModel>>(results);
        }

        [Fact]
        public void GetHasHttpGetAttribute()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributesOn(x => x.Get()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void GetByIdSendsActivityByActivityIdQueryWithCorrectData()
        {
            const int activityId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganization = new Organization() }});
            var sut = new ActivityApiController(mediator.Object);

            sut.Get(activityId);

            mediator.Verify(x => x.Send(It.Is<ActivityByActivityIdQuery>(y => y.ActivityId == activityId)));
        }

        [Fact]
        public void GetByIdReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganization = new Organization() }});
            var sut = new ActivityApiController(mediator.Object);
            var result = sut.Get(It.IsAny<int>());

            Assert.IsType<ActivityViewModel>(result);
        }

        //TODO: come back to these two tests until you hear back from Tony Surma about returning null instead of retruning HttpNotFound
        //GetByIdReturnsNullWhenActivityIsNotFoundById ???
        //[Fact]
        //public void GetByIdReturnsHttpNotFoundWhenActivityIsNotFoundById()
        //{
        //    var controller = new ActivityApiController(Mock.Of<IAllReadyDataAccess>(), null)
        //        .SetFakeUser("1");

        //    var result = controller.Get(It.IsAny<int>());
        //    Assert.IsType<HttpNotFoundResult>(result);
        //}

        [Fact]
        public void GetByIdHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}");
        }

        [Fact]
        public void GetByIdHasProducesAttributeWithCorrectContentTypes()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Type, typeof(ActivityViewModel));
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }

        [Fact]
        public void GetActivitiesByPostalCodeSendsAcitivitiesByPostalCodeQueryWithCorrectPostalCodeAndDistance()
        {
            const string zip = "zip";
            const int miles = 100;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AcitivitiesByPostalCodeQuery>())).Returns(new List<Activity>());

            var sut = new ActivityApiController(mediator.Object);
            sut.GetActivitiesByPostalCode(zip, miles);

            mediator.Verify(x => x.Send(It.Is<AcitivitiesByPostalCodeQuery>(y => y.PostalCode == zip && y.Distance == miles)));
        }

        [Fact]
        public void GetActivitiesByPostalCodeReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AcitivitiesByPostalCodeQuery>())).Returns(new List<Activity>());

            var sut = new ActivityApiController(mediator.Object);
            var result = sut.GetActivitiesByPostalCode(It.IsAny<string>(), It.IsAny<int>());

            Assert.IsType<List<ActivityViewModel>>(result);
        }

        [Fact]
        public void GetActivitiesByPostalCodeHasRouteAttributeWithRoute()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetActivitiesByPostalCode(It.IsAny<string>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "search");
        }

        [Fact]
        public void GetActivitiesByGeographySendsActivitiesByGeographyQueryWithCorrectLatitudeLongitudeAndMiles()
        {
            const double latitude = 1;
            const double longitude = 2;
            const int miles = 100;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivitiesByGeographyQuery>())).Returns(new List<Activity>());

            var sut = new ActivityApiController(mediator.Object);
            sut.GetActivitiesByGeography(latitude, longitude, miles);

            mediator.Verify(x => x.Send(It.Is<ActivitiesByGeographyQuery>(y => y.Latitude == latitude && y.Longitude == longitude && y.Miles == miles)));
        }

        [Fact]
        public void GetActivitiesByGeographyReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivitiesByGeographyQuery>())).Returns(new List<Activity>());

            var sut = new ActivityApiController(mediator.Object);
            var result = sut.GetActivitiesByGeography(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>());

            Assert.IsType<List<ActivityViewModel>>(result);
        }

        [Fact]
        public void GetActivitiesByLocationHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetActivitiesByGeography(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "searchbylocation");
        }

        [Fact]
        public void GetQrCodeHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetQrCode(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/qrcode");
        }

        [Fact]
        public void GetCheckinReturnsHttpNotFoundWhenUnableToFindActivityByActivityId()
        {
            var sut = new ActivityApiController(Mock.Of<IMediator>());
            var result = sut.GetCheckin(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void GetCheckinReturnsTheCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganization = new Organization() } });

            var sut = new ActivityApiController(mediator.Object);
            var result = (ViewResult)sut.GetCheckin(It.IsAny<int>());

            Assert.IsType<Activity>(result.ViewData.Model);
        }

        [Fact]
        public void GetCheckinReturnsTheCorrectView()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(new Activity { Campaign = new Campaign { ManagingOrganization = new Organization() }});

            var sut = new ActivityApiController(mediator.Object);
            var result = (ViewResult)sut.GetCheckin(It.IsAny<int>());

            Assert.Equal("NoUserCheckin", result.ViewName);
        }

        [Fact]
        public void GetCheckinHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributesOn(x => x.GetCheckin(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/checkin");
        }

        [Fact]
        public async Task PutCheckinReturnsHttpNotFoundWhenUnableToFindActivityByActivityId()
        {
            var sut = new ActivityApiController(Mock.Of<IMediator>());
            var result = await sut.PutCheckin(It.IsAny<int>());

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task PutCheckinSendsActivityByActivityIdQueryWithCorrectActivityId()
        {
            const int activityId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(new Activity());

            var sut = new ActivityApiController(mediator.Object);
            await sut.PutCheckin(activityId);

            mediator.Verify(x => x.Send(It.Is<ActivityByActivityIdQuery>(y => y.ActivityId == activityId)), Times.Once);
        }

        [Fact]
        public async Task PutCheckinSendsAddActivitySignupCommandAsyncWithCorrectDataWhenUsersSignedUpIsNotNullAndCheckinDateTimeIsNull()
        {
            const string userId = "userId";
            var utcNow = DateTime.UtcNow;

            var activity = new Activity();
            var activitySignup = new ActivitySignup { User = new ApplicationUser { Id = userId } };
            activity.UsersSignedUp.Add(activitySignup);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(activity);

            var sut = new ActivityApiController(mediator.Object) { DateTimeUtcNow = () => utcNow }
                .SetFakeUser(userId);
            await sut.PutCheckin(It.IsAny<int>());

            mediator.Verify(x => x.SendAsync(It.Is<AddActivitySignupCommandAsync>(y => y.ActivitySignup == activitySignup)));
            mediator.Verify(x => x.SendAsync(It.Is<AddActivitySignupCommandAsync>(y => y.ActivitySignup.CheckinDateTime == utcNow)));
        }

        [Fact]
        public async Task PutCheckinReturnsCorrectJsonWhenUsersSignedUpIsNotNullAndCheckinDateTimeIsNull()
        {
            const string userId = "userId";

            var activity = new Activity { Name = "ActivityName", Description = "ActivityDescription" };
            var activitySignup = new ActivitySignup { User = new ApplicationUser { Id = userId } };
            activity.UsersSignedUp.Add(activitySignup);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(activity);

            var sut = new ActivityApiController(mediator.Object)
                .SetFakeUser(userId);

            var expected = $"{{ Activity = {{ Name = {activity.Name}, Description = {activity.Description} }} }}";

            var result = (JsonResult)await sut.PutCheckin(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, result.Value.ToString());
        }

        [Fact]
        public async Task PutCheckinReturnsCorrectJsonWhenUsersSignedUpIsNullAndCheckinDateTimeIsNotNull()
        {
            const string userId = "userId";
            var activity = new Activity { Name = "ActivityName", Description = "ActivityDescription" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivityByActivityIdQuery>())).Returns(activity);

            var sut = new ActivityApiController(mediator.Object)
                .SetFakeUser(userId);

            var expected = $"{{ NeedsSignup = True, Activity = {{ Name = {activity.Name}, Description = {activity.Description} }} }}";

            var result = (JsonResult)await sut.PutCheckin(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, result.Value.ToString());
        }

        [Fact]
        public void PutCheckinHasHttpPutAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null);
            var attribute = (HttpPutAttribute)sut.GetAttributesOn(x => x.PutCheckin(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(HttpPutAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/checkin");
        }

        [Fact]
        public void PutCheckinHasAuthorizeAttribute()
        {
            var sut = new ActivityApiController(null);
            var attribute = (AuthorizeAttribute)sut.GetAttributesOn(x => x.PutCheckin(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(AuthorizeAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RegisterActivityReturnsHttpBadRequetWhenSignupModelIsNull()
        {
            var sut = new ActivityApiController(null);
            var result = await sut.RegisterActivity(null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RegisterActivityReturnsCorrectJsonWhenModelStateIsNotValid()
        {
            const string modelStateErrorMessage = "modelStateErrorMessage";

            var sut = new ActivityApiController(null);
            sut.AddModelStateError(modelStateErrorMessage);

            var jsonResult = (JsonResult)await sut.RegisterActivity(new ActivitySignupViewModel());
            var result = jsonResult.GetValueForProperty<List<string>>("errors");

            Assert.IsType<JsonResult>(jsonResult);
            Assert.IsType<List<string>>(result);
            Assert.Equal(result.First(), modelStateErrorMessage);
        }

        [Fact]
        public async Task RegisterActivitySendsActivitySignupCommandAsyncWithCorrectData()
        {
            var model = new ActivitySignupViewModel();
            var mediator = new Mock<IMediator>();

            var sut = new ActivityApiController(mediator.Object);
            await sut.RegisterActivity(model);

            mediator.Verify(x => x.SendAsync(It.Is<ActivitySignupCommand>(command => command.ActivitySignup.Equals(model))));
        }

        [Fact]
        public async Task RegisterActivityReturnsHttpStatusResultOfOk()
        {
            var sut = new ActivityApiController(Mock.Of<IMediator>());
            var result = (HttpStatusCodeResult)await sut.RegisterActivity(new ActivitySignupViewModel());

            Assert.IsType<HttpStatusCodeResult>(result);
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Fact]
        public void RegisterActivityHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new ActivityApiController(null);
            var attribute = (ValidateAntiForgeryTokenAttribute)sut.GetAttributesOn(x => x.RegisterActivity(It.IsAny<ActivitySignupViewModel>())).SingleOrDefault(x => x.GetType() == typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterActivityHasHttpPostAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null);
            var attribute = (HttpPostAttribute)sut.GetAttributesOn(x => x.RegisterActivity(It.IsAny<ActivitySignupViewModel>())).SingleOrDefault(x => x.GetType() == typeof(HttpPostAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "signup");
        }

        [Fact]
        public async Task UnregisterActivityReturnsHttpNotFoundWhenUnableToGetActivitySignupByActivitySignupIdAndUserId()
        {
            var controller = new ActivityApiController(Mock.Of<IMediator>());
            controller.SetDefaultHttpContext();

            var result = await controller.UnregisterActivity(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task UnregisterActivitySendsActivitySignupByActivityIdAndUserIdQueryWithCorrectActivityIdAndUserId()
        {
            const int activityId = 1;
            const string userId = "1";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivitySignupByActivityIdAndUserIdQuery>()))
                .Returns(new ActivitySignup { Activity = new Activity(), User = new ApplicationUser() });

            var controller = new ActivityApiController(mediator.Object)
                .SetFakeUser(userId);

            await controller.UnregisterActivity(activityId);

            mediator.Verify(x => x.Send(It.Is<ActivitySignupByActivityIdAndUserIdQuery>(y => y.ActivityId == activityId && y.UserId == userId)));
        }

        [Fact]
        public async Task UnregisterActivityPublishesUserUnenrollsWithCorrectData()
        {
            const int activityId = 1;
            const string applicationUserId = "applicationUserId";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivitySignupByActivityIdAndUserIdQuery>()))
                .Returns(new ActivitySignup { Activity = new Activity { Id = activityId }, User = new ApplicationUser { Id = applicationUserId }});

            var controller = new ActivityApiController(mediator.Object);
            controller.SetDefaultHttpContext();
                    
            await controller.UnregisterActivity(activityId);

            mediator.Verify(mock => mock.PublishAsync(It.Is<UserUnenrolls>(ue => ue.ActivityId == activityId && ue.UserId == applicationUserId)), Times.Once);
        }

        [Fact]
        public async Task UnregisterActivitySendsDeleteActivityAndTaskSignupsCommandAsyncWithCorrectActivitySignupId()
        {
            const int activityId = 1;
            const int activitySignupId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActivitySignupByActivityIdAndUserIdQuery>()))
                .Returns(new ActivitySignup { Id = activitySignupId, Activity = new Activity(), User = new ApplicationUser() });

            var controller = new ActivityApiController(mediator.Object);
            controller.SetDefaultHttpContext();

            await controller.UnregisterActivity(activityId);

            mediator.Verify(x => x.SendAsync(It.Is<DeleteActivityAndTaskSignupsCommandAsync>(y => y.ActivitySignupId == activitySignupId)));
        }

        [Fact]
        public void UnregisterActivityHasHttpDeleteAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null);
            var attribute = (HttpDeleteAttribute)sut.GetAttributesOn(x => x.UnregisterActivity(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(HttpDeleteAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/signup");
        }

        [Fact]
        public void UnregisterActivityHasAuthorizeAttribute()
        {
            var sut = new ActivityApiController(null);
            var attribute = (AuthorizeAttribute)sut.GetAttributesOn(x => x.UnregisterActivity(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(AuthorizeAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/activity");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new ActivityApiController(null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }
    }
}

