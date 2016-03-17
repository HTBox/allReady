using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Xunit;

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
    }
}
