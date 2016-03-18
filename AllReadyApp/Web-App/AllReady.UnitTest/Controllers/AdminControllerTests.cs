using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc.Routing;

namespace AllReady.UnitTest.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        public void RegisterReturnsViewResult()
        {
            var sut = new AdminController(null, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            var result = sut.Register();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RegisterHasHttpGetAttribute()
        {
            var sut = new AdminController(null, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterHasAllowAnonymousAttribute()
        {
            var sut = new AdminController(null, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RegisterReturnsViewResultWhenModelStateIsNotValid()
        {
            var sut = new AdminController(null, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            sut.AddModelStateError();

            var result = await sut.Register(It.IsAny<RegisterViewModel>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterReturnsCorrectModelWhenModelStateIsNotValid()
        {
            var model = new RegisterViewModel();

            var sut = new AdminController(null, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            sut.AddModelStateError();

            var result = await sut.Register(model) as ViewResult;
            var modelResult = result.ViewData.Model as RegisterViewModel;

            Assert.IsType<RegisterViewModel>(modelResult);
            Assert.Same(model, modelResult);
        }

        [Fact]
        public async Task RegisterInvokesCreateAsyncWithCorrectUserAndPassword()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);

            await sut.Register(model);

            userManager.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone),
                model.Password));
        }

        [Fact]
        public async Task RegisterInvokesGenerateEmailConfirmationTokenAsyncWithCorrectUserWhenUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = Mock.Of<IUrlHelper>();

            await sut.Register(model);

            userManager.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone)));
        }

        [Fact]
        public async Task RegisterInvokesUrlActionWithCorrectParametersWhenUserCreationIsSuccessful()
        {
            const string requestScheme = "requestScheme";
            
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.Register(new RegisterViewModel());

            //note: I can't test the Values part here b/c I do not have control over the Id generation on ApplicationUser b/c it's new'ed up in the controller
            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ConfirmEmail" &&
                uac.Controller == "Admin" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task RegisterInvokesSendEmailAsyncWithCorrectParametersWhenUserCreationIsSuccessful()
        {
            const string callbackUrl = "callbackUrl";

            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);

            var emailSender = new Mock<IEmailSender>();

            var sut = new AdminController(userManager.Object, null, emailSender.Object, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.Register(model);

            emailSender.Verify(x => x.SendEmailAsync(model.Email, "Confirm your account", $"Please confirm your account by clicking this <a href=\"{callbackUrl}\">link</a>"));
        }

        [Fact]
        public async Task RegisterRedirectsToCorrectActionWhenUserCreationIsSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            var result = await sut.Register(new RegisterViewModel()) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(AdminController.DisplayEmail));
            Assert.Equal(result.ControllerName, "Admin");
        }

        [Fact]
        public async Task RegisterAddsIdentityResultErrorsToModelStateErrorsWhenUserCreationIsNotSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "IdentityErrorDescription" });

            var userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(identityResult));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());

            await sut.Register(new RegisterViewModel());

            var errorMessages = sut.ModelState.GetErrorMessages();

            Assert.Equal(errorMessages.Single(), identityResult.Errors.Select(x => x.Description).Single());
        }

        [Fact]
        public async Task RegisterReturnsViewResultAndCorrectModelWhenUserCreationIsNotSuccessful()
        {
            var model = new RegisterViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());

            var result = await sut.Register(model) as ViewResult;
            var modelResult = result.ViewData.Model as RegisterViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<RegisterViewModel>(modelResult);
            Assert.Same(model, modelResult);
        }
    }
}
