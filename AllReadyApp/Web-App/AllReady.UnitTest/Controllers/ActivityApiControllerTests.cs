using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.Extensions;
using AllReady.Features.Notifications;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc;

namespace AllReady.UnitTest.Controllers
{
    //GetActivitiesByZip
    //GetActivitiesByLocation
    //GetCheckin
    //PutCheckin
    //RegisterActivity
    //UnregisterActivity
    //    - existing test the needs tests for:
    //      - GetActivitySignup is made with correct Id and User.GetUserId
    //        - _mediator.PublishAsync and _allReadyDataAccess.DeleteActivityAndTaskSignupsAsync are called with correct args
    //GetQrCode: check notes for this one

    public class ActivityApiControllerTest : TestBase
    {
        private static IServiceProvider _serviceProvider;
        private static bool populatedData;
        private static int activitiesAdded;
        private Mock<IMediator> _mediator;

        public ActivityApiControllerTest()
        {
            if (_serviceProvider == null)
            {
                var services = new ServiceCollection();

                // Add EF (Full DB, not In-Memory)
                services.AddEntityFramework()
                    .AddInMemoryDatabase()
                    .AddDbContext<AllReadyContext>(options => options.UseInMemoryDatabase());

                // Setup hosting environment
                IHostingEnvironment hostingEnvironment = new HostingEnvironment();
                hostingEnvironment.EnvironmentName = "Development";
                services.AddSingleton(x => hostingEnvironment);
                _serviceProvider = services.BuildServiceProvider();
            }
        }


        //TODO: refactor to mediator
        //[Fact]
        //public void GetActivitiesByZip()
        //{
        //}
        //[Fact]
        //public void GetActivitiesByLocation()
        //{
        //}
        //[Fact]
        //public void GetCheckin()
        //{
        //}
        //async invocation of a method off of IAllReadyDataAcces on this one
        //[Fact]
        //public void PutCheckin()
        //{
        //}

        [Fact]
        public void GetReturnsActivitiesWitUnlockedCampaigns()
        {
            var activities = new List<Activity>
            {
                new Activity { Id = 1, Campaign = new Campaign { Locked = false }},
                new Activity { Id = 2, Campaign = new Campaign { Locked = true }}
            };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.Activities).Returns(activities);

            var sut = new ActivityApiController(dataAccess.Object, null);
            var results = sut.Get().ToList();
            
            Assert.Equal(activities[0].Id, results[0].Id);
        }

        [Fact]
        public void GetReturnsCorrectModel()
        {
            var sut = new ActivityApiController(Mock.Of<IAllReadyDataAccess>(), null);
            var results = sut.Get().ToList();
            Assert.IsType<List<ActivityViewModel>>(results);
        }

        //TODO
        //GetByIdReturnsHttpNotFoundWhenActivityIsNotFoundById
        //GetByIdReturnsNullWhenActivityIsNotFoundById ???
        //GetByIdReturnsCorrectViewModel

        [Fact]
        public void GetSingleActivity()
        {
            var controller = GetActivityApiController();

            const int recordId = 5;
            var activityViewModel = controller.Get(recordId);

            Assert.Equal(activityViewModel.Id, recordId);
            Assert.Equal(activityViewModel.CampaignName, string.Format(TestActivityModelProvider.CampaignNameFormat, recordId));
            Assert.Equal(activityViewModel.CampaignId, recordId);
            Assert.Equal(activityViewModel.Description, string.Format(TestActivityModelProvider.ActivityDescriptionFormat, recordId));
            Assert.Equal(activityViewModel.EndDateTime, DateTime.MaxValue.ToUniversalTime());
            Assert.Equal(activityViewModel.StartDateTime, DateTime.MinValue.ToUniversalTime());
        }

        [Fact]
        public void ActivityDoesExist()
        {
            var controller = GetActivityApiController();
            var activityViewModel = controller.Get(1);
            Assert.NotNull(activityViewModel);
        }

        [Fact]
        public void HandlesInvalidActivityId()
        {
            var controller = GetActivityApiController();
            var activityViewModel = controller.Get(-1);
            Assert.Null(activityViewModel);
        }

        [Fact]
        public async Task UnregisterActivityReturnsHttpNotFoundWhenUnableToGetActivitySignupByActivitySignupIdAndUserId()
        {
            var controller = new ActivityApiController(Mock.Of<IAllReadyDataAccess>(), null)
                .SetFakeUser("1");

            var result = await controller.UnregisterActivity(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task UnregisterActivityGetActivitySignUpIsCalledWithCorrectActivityIdAndUserId()
        {
            const int activityId = 1;
            const string userId = "1";
            
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivitySignup(It.IsAny<int>(), It.IsAny<string>())).Returns(new ActivitySignup { Activity = new Activity(), User = new ApplicationUser() });

            var controller = new ActivityApiController(dataAccess.Object, Mock.Of<IMediator>())
                .SetFakeUser(userId);
            
            await controller.UnregisterActivity(activityId);

            dataAccess.Verify(x => x.GetActivitySignup(activityId, userId), Times.Once);
        }

        [Fact]
        public async Task UnregisterActivityPublishesUserUnenrollsWithCorrectData()
        {
            const int activityId = 1;
            const string applicationUserId = "applicationUserId";

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess
                .Setup(x => x.GetActivitySignup(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new ActivitySignup
                {
                    Activity = new Activity { Id = activityId },
                    User = new ApplicationUser { Id = applicationUserId }
                });

            var mediator = new Mock<IMediator>();

            var controller = new ActivityApiController(dataAccess.Object, mediator.Object)
                .SetFakeUser("1");

            await controller.UnregisterActivity(activityId);

            mediator.Verify(mock => mock.PublishAsync(It.Is<UserUnenrolls>(ue => ue.ActivityId == activityId && ue.UserId == applicationUserId)), Times.Once);
        }

        [Fact]
        public async Task UnregisterActivityDeleteActivityAndTaskSignupsAsyncIsCalledWithCorrectActivitySignupId()
        {
            const int activitySignupId = 1;

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess
                .Setup(x => x.GetActivitySignup(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new ActivitySignup
                {
                    Id = activitySignupId,
                    Activity = new Activity (),
                    User = new ApplicationUser()
                });

            var controller = new ActivityApiController(dataAccess.Object, Mock.Of<IMediator>())
                .SetFakeUser("1");

            await controller.UnregisterActivity(2);

            dataAccess.Verify(x => x.DeleteActivityAndTaskSignupsAsync(activitySignupId), Times.Once);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/activity");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }

        [Fact]
        public void GetHasHttpGetAttribute()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Get()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void GetByIdHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}");
        }

        [Fact]
        public void GetByIdHasProducesAttributeWithCorrectContentTypes()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Type, typeof(ActivityViewModel));
            Assert.Equal(attribute.ContentTypes.Select(x => x.MediaType).First(), "application/json");
        }

        [Fact]
        public void GetActivitiesByZipHasRouteAttributeWithRoute()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetActivitiesByZip(It.IsAny<string>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "search");
        }

        [Fact]
        public void GetActivitiesByLocationHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetActivitiesByLocation(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "searchbylocation");
        }

        [Fact]
        public void GetQrCodeHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetQrCode(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/qrcode");
        }

        [Fact]
        public void GetCheckinHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.GetCheckin(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/checkin");
        }

        [Fact]
        public void PutCheckinHasHttpPutAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = (HttpPutAttribute)sut.GetAttributesOn(x => x.PutCheckin(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(HttpPutAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/checkin");
        }

        [Fact]
        public void PutCheckinHasAuthorizeAttribute()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = (AuthorizeAttribute)sut.GetAttributesOn(x => x.PutCheckin(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(AuthorizeAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterActivityHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = (ValidateAntiForgeryTokenAttribute)sut.GetAttributesOn(x => x.RegisterActivity(It.IsAny<ActivitySignupViewModel>())).SingleOrDefault(x => x.GetType() == typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterActivityHasHttpPostAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = (HttpPostAttribute)sut.GetAttributesOn(x => x.RegisterActivity(It.IsAny<ActivitySignupViewModel>())).SingleOrDefault(x => x.GetType() == typeof(HttpPostAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "signup");
        }

        [Fact]
        public void UnregisterActivityHasHttpDeleteAttributeWithCorrectTemplate()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = (HttpDeleteAttribute)sut.GetAttributesOn(x => x.UnregisterActivity(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(HttpDeleteAttribute));
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{id}/signup");
        }

        [Fact]
        public void UnregisterActivityHasAuthorizeAttribute()
        {
            var sut = new ActivityApiController(null, null);
            var attribute = (AuthorizeAttribute)sut.GetAttributesOn(x => x.UnregisterActivity(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(AuthorizeAttribute));
            Assert.NotNull(attribute);
        }

        #region old tests the really test Respository code

        #endregion

        #region Helper Methods

        private ActivityApiController GetActivityApiController()
        {
            var allReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var allReadyDataAccess = new AllReadyDataAccessEF7(allReadyContext);

            _mediator = new Mock<IMediator>();
            var controller = new ActivityApiController(allReadyDataAccess, _mediator.Object);

            PopulateData(allReadyContext);
            return controller;
        }

        private static void PopulateData(DbContext context)
        {
            if (!populatedData)
            {
                var activities = TestActivityModelProvider.GetActivities();

                foreach (var activity in activities)
                {
                    context.Add(activity);
                    context.Add(activity.Campaign);
                    activitiesAdded++;
                }
                context.SaveChanges();
                populatedData = true;
            }
        }

        private class TestActivityModelProvider
        {
            public const string CampaignNameFormat = "Campaign {0}";
            public const string CampaignDescriptionFormat = "Description for campaign {0}";
            public const string OrganizationNameFormat = "Test Organization {0}";
            public const string ActivityNameFormat = "Activity {0}";
            public const string ActivityDescriptionFormat = "Description for activity {0}";
            public const string UserNameFormat = "User {0}";
            public const string TaskDescriptionFormat = "Task {0}";

            public static IEnumerable<Activity> GetActivities()
            {
                var users = Enumerable.Range(1, 10).Select(n =>
                    new ApplicationUser()
                    {
                        Id = n.ToString(),
                        Name = string.Format(UserNameFormat, n)
                    }).ToArray();

                var organizations = Enumerable.Range(1, 10).Select(n =>
                    new Organization()
                    {
                        Id = n,
                        Name = string.Format(OrganizationNameFormat, n)
                    }).ToArray();

                var campaigns = Enumerable.Range(1, 10).Select(n =>
                    new Campaign()
                    {
                        Description = string.Format(CampaignDescriptionFormat, n),
                        Name = string.Format(CampaignNameFormat, n),
                        Id = n,
                        ManagingOrganization = organizations[n - 1]
                    }).ToArray();

                var activities = Enumerable.Range(1, 10).Select(n =>
                    new Activity()
                    {
                        Campaign = campaigns[n - 1],
                        EndDateTime = DateTime.MaxValue.ToUniversalTime(),
                        StartDateTime = DateTime.MinValue.ToUniversalTime(),
                        Name = string.Format(ActivityNameFormat, n),
                        Description = string.Format(ActivityDescriptionFormat, n),
                        Id = n,
                        UsersSignedUp = new List<ActivitySignup>()
                        {
                            new ActivitySignup()
                            {
                                User = users[n - 1],
                                SignupDateTime = DateTime.Now.ToUniversalTime(),
                                PreferredEmail = "foo@foo.com",
                                PreferredPhoneNumber = "(555) 555-5555"
                            }
                        },
                        Tasks = new List<AllReadyTask>()
                        {
                            new AllReadyTask()
                            {
                                Id = n,
                                Name = string.Format(TaskDescriptionFormat,n),
                                NumberOfVolunteersRequired = 1,
                                Organization = organizations[n - 1],
                                AssignedVolunteers = new List<TaskSignup>()
                                {
                                    new TaskSignup()
                                    {
                                        User = users[n - 1],
                                        StatusDateTimeUtc = DateTime.Now.ToUniversalTime(),
                                        Status = "Ready To Rock And Roll"
                                    }
                                }
                            }
                        }
                    }).ToArray();
                return activities;
            }
        }
        #endregion
    }
}

