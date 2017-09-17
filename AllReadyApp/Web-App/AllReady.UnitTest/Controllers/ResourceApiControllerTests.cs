using AllReady.Controllers;
using AllReady.Models;
using System.Collections.Generic;
using Moq;
using Xunit;
using System.Linq;
using AllReady.Features.Resource;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels;
using AllReady.ViewModels.Resource;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.UnitTest.Controllers
{
    public class ResourceApiControllerTests
    {
        [Fact]
        public void GetResourcesByCategorySendsResourcesByCategoryQueryWithCorrectData()
        {
            const string category = "category";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ResourcesByCategoryQuery>())).Returns(new List<Resource>());

            var sut = new ResourceApiController(mediator.Object);
            sut.GetResourcesByCategory(category);

            mediator.Verify(x => x.Send(It.Is<ResourcesByCategoryQuery>(y => y.Category == category)));
        }

        [Fact]
        public void GetResourcesByCategoryReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ResourcesByCategoryQuery>())).Returns(new List<Resource> { new Resource() });

            var sut = new ResourceApiController(mediator.Object);
            var results = sut.GetResourcesByCategory(It.IsAny<string>());

            Assert.IsType<List<ResourceViewModel>>(results);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectTemplate()
        {
            var sut = new ResourceApiController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("api/resource", attribute.Template);
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new ResourceApiController(null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("application/json", attribute.ContentTypes.Select(x => x).First());
        }
    }
}
