using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNet.Http;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc;
using System.Linq;
using System;
using AllReady.Extensions;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class CampaignAdminControllerTests : InMemoryContextTest
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact()]
        public void IndexSendsCampaignListQueryWithCorrectDataWhenUserIsOrgAdmin()
        {
            Organization aginAware = AgincourtAwarenessExtended(Context);
            Organization pwPrepares = ParkwoodsPreparesExtended(Context);
            Context.SaveChanges();

            // Org admin should only see own campaigns
            CampaignController controller = CampaignControllerWithListQuery(Context, UserType.OrgAdmin.ToString(), aginAware.Id);
            ViewResult view = (ViewResult)controller.Index();
            IEnumerable<AllReady.Areas.Admin.Models.CampaignSummaryModel> viewModel = (IEnumerable<AllReady.Areas.Admin.Models.CampaignSummaryModel>)view.ViewData.Model;
            Assert.Equal(viewModel.Where(t => t.OrganizationId == aginAware.Id).Count(), aginAware.Campaigns.Count());
        }


        [Fact()]
        public void IndexSendsCampaignListQueryWithCorrectDataWhenUserIsNotOrgAdmin()
        {
            Organization aginAware = AgincourtAwarenessExtended(Context);
            Organization pwPrepares = ParkwoodsPreparesExtended(Context);
            Context.SaveChanges();

            // Site admin should see all campaigns
            CampaignController controller = CampaignControllerWithListQuery(Context, UserType.SiteAdmin.ToString(), null);
            ViewResult view = (ViewResult)controller.Index();      
            IEnumerable<AllReady.Areas.Admin.Models.CampaignSummaryModel> viewModel = (IEnumerable<AllReady.Areas.Admin.Models.CampaignSummaryModel>)view.ViewData.Model;
            Assert.Equal(viewModel.Count(), Context.Campaigns.Count());
        }

        [Fact(Skip = "NotImplemented")]
        public void IndexReturnsCorrectViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsSendsCampaignDetailQueryWithCorrectCampaignId()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenVieModelIsNull()
        {
            CampaignController controller;
            MockMediatorCampaignDetailQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultIfUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithDetailQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewWhenViewModelIsNotNullAndUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithDetailQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsReturnsCorrectViewModelWhenViewModelIsNotNullAndUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void CreateReturnsCorrectViewWithCorrectViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditPostReturnsBadRequestWhenCampaignIsNull()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var result = await controller.Edit(null, null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task EditPostAddsCorrectKeyAndErrorMessageToModelStateWhenCampaignEndDateIsLessThanCampainStartDate()
        {
            var campaignSummaryModel = new CampaignSummaryModel { OrganizationId = 1, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(-1)};

            var sut = new CampaignController(null, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, campaignSummaryModel.OrganizationId.ToString())
            });

            await sut.Edit(campaignSummaryModel, null);
            var modelStateErrorCollection = sut.ModelState.GetErrorMessagesByKey(nameof(CampaignSummaryModel.EndDate));

            Assert.Equal(modelStateErrorCollection.Single().ErrorMessage, "The end date must fall on or after the start date.");
        }

        [Fact]
        public async Task EditPostInsertsCampaign()
        {
            Location bogusAve = BogusAve_entity();
            Context.Locations.Add(bogusAve);
            Organization aginAware = AgincourtAware_entity();
            aginAware.Location = bogusAve;
            Context.Organizations.Add(aginAware);
            Context.SaveChanges();

            var model = MassiveTrafficLightOutage_model;
            model.OrganizationId = aginAware.Id;

            var controller = CampaignControllerWithSummaryModel(model, Context, UserType.OrgAdmin.ToString());
            var file = FormFile("image/jpeg");
            await controller.Edit(model, file);
            Assert.Single(Context.Campaigns.Where(t => t.Name == model.Name));
        }

        [Fact]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            var result = await controller.Edit(new CampaignSummaryModel { OrganizationId = It.IsAny<int>() }, null);
            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectActionWithCorrectRouteValuesWhenModelStateIsValid()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var result = await controller.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, null);

            //TODO: test result for correct Action name and Route values
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task EditPostAddsErrorToModelStateWhenInvalidImageFormatIsSupplied()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var file = FormFile("");

            await controller.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, file);

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("ImageUrl"));
            //TODO: test that the value associated with the key is correct
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModelWhenInvalidImageFormatIsSupplied()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task EditPostUploadsImageToImageService()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(mock => mock.UploadCampaignImageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IFormFile>())).Returns(() => Task.FromResult("")).Verifiable();

            var sut = new CampaignController(mockMediator.Object, mockImageService.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            var file = FormFile("image/jpeg");

            await sut.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = organizationId, Id = campaignId}, file);

            mockImageService.Verify(mock => mock.UploadCampaignImageAsync(
                        It.Is<int>(i => i == organizationId),
                        It.Is<int>(i => i == campaignId),
                It.Is<IFormFile>(i => i == file)), Times.Once);
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasValidateAntiForgeryTokenttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenCampaignIsNotFound()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewWhenUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        public async Task DeleteConfirmedSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new CampaignController(mediator.Object, null);
            await sut.DeleteConfirmed(campaignId);

            mediator.Verify(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(i => i.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await controller.DeleteConfirmed(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailConfirmedSendsDeleteCampaignCommandWithCorrectCampaignIdWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryModel { OrganizationId = organizationId });

            var sut = new CampaignController(mockMediator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            await sut.DeleteConfirmed(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<DeleteCampaignCommand>(i => i.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryModel { OrganizationId = organizationId });

            var sut = new CampaignController(mockMediator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            var routeValues = new Dictionary<string, object> { ["area"] = "Admin" };

            var result = await sut.DeleteConfirmed(campaignId) as RedirectToActionResult;
            Assert.Equal(result.ActionName, nameof(CampaignController.Index));
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LockUnlockReturnsHttpUnauthorizedResultWhenUserIsNotSiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LockUnlockSendsLockUnlockCampaignCommandWithCorrectCampaignIdWhenUserIsSiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LockUnlockRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsSiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void LockUnlockHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LockUnlockdHasValidateAntiForgeryTokenAttribute()
        {
        }

        #region Helper Methods
        private static Mock<IMediator> MockMediatorCampaignDetailQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(null).Verifiable();

            var mockImageService = new Mock<IImageService>();

            controller = new CampaignController(mockMediator.Object, mockImageService.Object);

            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorCampaignSummaryQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(null).Verifiable();

            var mockImageService = new Mock<IImageService>();
            controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            return mockMediator;
        }

        private static CampaignController CampaignControllerWithDetailQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(new CampaignDetailModel { OrganizationId = organizationId }).Verifiable();

            var mockImageService = new Mock<IImageService>();

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            controller.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return controller;
        }

        private static CampaignController CampaignControllerWithSummaryQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryModel { OrganizationId = organizationId, Location = new LocationEditModel() }).Verifiable();

            var mockImageService = new Mock<IImageService>();

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            controller.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return controller;
        }

        private static CampaignController CampaignControllerWithSummaryModel(CampaignSummaryModel model, AllReadyContext Context, string userType)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditCampaignCommand>()))
                .Returns((EditCampaignCommand command) => {
                    IAsyncRequestHandler<EditCampaignCommand, int> handler = new EditCampaignCommandHandler(Context);
                    return handler.Handle(command);
                });

            var mockImageService = new Mock<IImageService>();

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            controller.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, model.OrganizationId.ToString())
            });

            return controller;
        }
        private static CampaignController CampaignControllerWithListQuery(AllReadyContext Context, string UserType, int? UserOrgId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<CampaignListQuery>()))
                .Returns((CampaignListQuery q) => {
                    IRequestHandler<CampaignListQuery, IEnumerable<CampaignSummaryModel>> handler = new CampaignListQueryHandler(Context);
                    return handler.Handle(q);
                });

            var mockImageService = new Mock<IImageService>();
            CampaignController controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            List<Claim> claims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType),
            };
            if (UserOrgId.HasValue)
                claims.Add(new Claim(AllReady.Security.ClaimTypes.Organization, UserOrgId.ToString()));
            controller.SetClaims(claims);
            return controller;
        }

        private static IFormFile FormFile(string fileType)
        {
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(mock => mock.ContentType).Returns(fileType);
            return mockFormFile.Object;
        }
        #endregion

        #region "Test Models"
        public static LocationEditModel BogusAve_model {
            get {
                return new LocationEditModel() {
                    Address1 = "25 Bogus Ave",
                    City = "Agincourt",
                    State = "Ontario",
                    Country = "Canada",
                    PostalCode = "M1T2T9"
                };
            }
        }
        public static OrganizationEditModel AgincourtAware_model {
            get {
                return new OrganizationEditModel() {
                    Name = "Agincourt Awareness",
                    Location = BogusAve_model,
                    WebUrl = "http://www.AgincourtAwareness.ca",
                    LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" };
            }
        }
        public static CampaignSummaryModel MassiveTrafficLightOutage_model {
            get {
                return new CampaignSummaryModel() {
                    Description = "Preparations to be ready to deal with a wide-area traffic outage.",
                    EndDate = DateTime.Today.AddMonths(1),
                    ExternalUrl = "http://agincourtaware.trafficlightoutage.com",
                    ExternalUrlText = "Agincourt Aware: Traffic Light Outage",
                    Featured = false,
                    FileUpload = null,
                    FullDescription = "<h1><strong>Massive Traffic Light Outage Plan</strong></h1>\r\n<p>The Massive Traffic Light Outage Plan (MTLOP) is the official plan to handle a major traffic light failure.</p>\r\n<p>In the event of a wide-area traffic light outage, an alternative method of controlling traffic flow will be necessary. The MTLOP calls for the recruitment and training of volunteers to be ready to direct traffic at designated intersections and to schedule and follow-up with volunteers in the event of an outage.</p>",
                    Id = 0,
                    ImageUrl = null,
                    Location = BogusAve_model,
                    Locked = false,
                    Name = "Massive Traffic Light Outage Plan",
                    PrimaryContactEmail = "mlong@agincourtawareness.com",
                    PrimaryContactFirstName = "Miles",
                    PrimaryContactLastName = "Long",
                    PrimaryContactPhoneNumber = "416-555-0119",
                    StartDate = DateTime.Today,
                    TimeZoneId = "Eastern Standard Time",
                };
            }
        }
        #endregion

        #region "TestEntities"
        public static Campaign MassiveTrafficLightOutage_entity() {
            DateTime start = DateTime.Today;
            string TimeZoneId = "Eastern Standard Time";
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            TimeSpan startDateTimeOffset = timeZoneInfo.GetUtcOffset(start);
            DateTimeOffset StartDateTime = new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, startDateTimeOffset);
            DateTimeOffset EndDateTime = StartDateTime.AddMonths(1);
            Campaign ret = new Campaign()
            {
                Description = "Preparations to be ready to deal with a wide-area traffic outage.",
                EndDateTime = EndDateTime,
                ExternalUrl = "http://www.agincourtaware.com/trafficlightoutage",
                ExternalUrlText = "Agincourt Aware: Traffic Light Outage",
                Featured = false,
                FullDescription = "<h1><strong>Massive Traffic Light Outage Plan</strong></h1>\r\n<p>The Massive Traffic Light Outage Plan (MTLOP) is the official plan to handle a major traffic light failure.</p>\r\n<p>In the event of a wide-area traffic light outage, an alternative method of controlling traffic flow will be necessary. The MTLOP calls for the recruitment and training of volunteers to be ready to direct traffic at designated intersections and to schedule and follow-up with volunteers in the event of an outage.</p>",
                Id = 0,
                ImageUrl = null,
                Locked = false,
                Name = "Massive Traffic Light Outage Plan",
                StartDateTime = StartDateTime,
                TimeZoneId = TimeZoneId,
            };
            return ret;
        }
        public static Campaign FirePreventionDays_entity()
        {
            DateTime start = DateTime.Today;
            string TimeZoneId = "Eastern Standard Time";
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            TimeSpan startDateTimeOffset = timeZoneInfo.GetUtcOffset(start);
            DateTimeOffset StartDateTime = new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, startDateTimeOffset);
            DateTimeOffset EndDateTime = StartDateTime.AddMonths(1);
            Campaign ret = new Campaign()
            {
                Description = "Activity days to prevent and be able to respond to a fire.",
                EndDateTime = EndDateTime,
                ExternalUrl = "http://www.agincourtaware.com/firepreventiondays",
                ExternalUrlText = "Agincourt Aware: Neighbourhood Fire Prevention Days",
                Featured = false,
                FullDescription = "<h1><strong>Neighbourhood Fire Prevention Days</strong></h1>\r\n<p>The focus of this year's Fire Prevention Days is getting the message out about smoke alarms. Smoke alarms save lives and need to be replaced every 10 years.</p>\r\n<p>Activities will center around educating the public and making it easy to install/replace fire alarms.</p>",
                Id = 0,
                ImageUrl = null,
                Locked = false,
                Name = "Neighbourhood Fire Prevention Days",
                StartDateTime = StartDateTime,
                TimeZoneId = TimeZoneId,
            };
            return ret;
        }
        public static Campaign SafetyKit_entity()
        {
            DateTime start = DateTime.Today;
            string TimeZoneId = "Eastern Standard Time";
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            TimeSpan startDateTimeOffset = timeZoneInfo.GetUtcOffset(start);
            DateTimeOffset StartDateTime = new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, startDateTimeOffset);
            DateTimeOffset EndDateTime = StartDateTime.AddMonths(1);
            Campaign ret = new Campaign()
            {
                Description = "A push to inform the public about the importance of preparing a safety kit and what to have in it.",
                EndDateTime = EndDateTime,
                ExternalUrl = "http://www.ParkwoodsPrepares.com/SafetyKit",
                ExternalUrlText = "Simple Safety Kit Building - Brought to You by Parkwoods Prepares",
                Featured = false,
                FullDescription = "<p>A disaster supplies kit is simply a collection of basic items your household may need in the event of an emergency.</p><p> Try to assemble your kit well in advance of an emergency.You may have to evacuate at a moment’s notice and take essentials with you.You will probably not have time to search for the supplies you need or shop for them.</p><p> You may need to survive on your own after an emergency.This means having your own food, water and other supplies in sufficient quantity to last for at least 72 hours.Local officials and relief workers will be on the scene after a disaster but they cannot reach everyone immediately.You could get help in hours or it might take days.</p><p> Additionally, basic services such as electricity, gas, water, sewage treatment and telephones may be cut off for days or even a week, or longer.Your supplies kit should contain items to help you manage during these outages.</p>",
                Id = 0,
                ImageUrl = null,
                Locked = false,
                Name = "Simple Safety Kit Building",
                StartDateTime = StartDateTime,
                TimeZoneId = TimeZoneId,
            };
            return ret;
        }

        public static Location BogusAve_entity()
        {
            return new Location()
            {
                Address1 = "25 Bogus Ave",
                City = "Agincourt",
                State = "Ontario",
                Country = "Canada",
                PostalCode = "M1T 2T9"
            };
        }
        public static Location ShamStreet_entity()
        {
            return new Location()
            {
                Address1 = "173 Sham Street",
                City = "",
                State = "Ontario",
                Country = "Canada",
                PostalCode = "M6J 2W6"
            };
        }
        public static Organization AgincourtAware_entity() {
            return new Organization()
            {
                Name = "Agincourt Awareness",
                WebUrl = "http://www.AgincourtAwareness.ca",
                LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png"
            };
        }
        public static Organization ParkwoodsPrepares_entity() {
            return new Organization()
            {
                Name = "Parkwoods Prepares",
                WebUrl = "http://www.ParkwoodsPrepares.ca",
                LogoUrl = "http://www.ParkwoodsPrepares.ca/images/PPLogo.png"
            };
        }
        private static Organization ParkwoodsPreparesExtended(AllReadyContext InMemoryContext)
        {
            Organization pwPrep = ParkwoodsPrepares_entity();
            Location newLoc = ShamStreet_entity();
            InMemoryContext.Locations.Add(newLoc);
            pwPrep.Location = newLoc;
            InMemoryContext.Organizations.Add(pwPrep);

            Campaign safetyKit = SafetyKit_entity();
            newLoc = ShamStreet_entity(); // Same address as the organization but different instance
            InMemoryContext.Locations.Add(newLoc);
            safetyKit.Location = newLoc;
            safetyKit.ManagingOrganization = pwPrep;
            InMemoryContext.Campaigns.Add(safetyKit);

            return pwPrep;
        }

        private static Organization AgincourtAwarenessExtended(AllReadyContext InMemoryContext)
        {
            Organization aginAware = AgincourtAware_entity();
            Location newLoc = BogusAve_entity();
            InMemoryContext.Locations.Add(newLoc);
            aginAware.Location = newLoc;
            InMemoryContext.Organizations.Add(aginAware);

            Campaign lightOut = MassiveTrafficLightOutage_entity();
            newLoc = BogusAve_entity(); // Same address as the organization but different instance
            InMemoryContext.Locations.Add(newLoc);
            lightOut.Location = newLoc;
            lightOut.ManagingOrganization = aginAware;
            InMemoryContext.Campaigns.Add(lightOut);

            Campaign firePrevent = FirePreventionDays_entity();
            newLoc = BogusAve_entity(); // Same address as the organization but different instance
            InMemoryContext.Locations.Add(newLoc);
            firePrevent.Location = newLoc;
            firePrevent.ManagingOrganization = aginAware;
            InMemoryContext.Campaigns.Add(firePrevent);

            return aginAware;
        }
        #endregion
    }

}
