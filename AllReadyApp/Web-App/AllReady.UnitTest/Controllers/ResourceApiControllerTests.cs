using AllReady.Controllers;
using AllReady.Models;
using AllReady.ViewModels;
using System.Collections.Generic;
using Moq;
using Xunit;
using System.Linq;

namespace AllReady.UnitTest.Controllers
{
    public class ResourceApiControllerTests : TestBase
    {
        [Fact]
        public void GetResourcesByCategory()
        {
            //Arrange
            var mockAllReadyDataAccess = new Mock<IAllReadyDataAccess>();

            string resourceCat1 = "1", resourceCat2 = "2";
            var cat1Resources = new List<Resource>()
            {
                new Resource() { Id = 1 }
            };

            var cat2Resources = new List<Resource>()
            {
                new Resource() { Id = 2 }
            };

            mockAllReadyDataAccess.Setup(x => x.GetResourcesByCategory(resourceCat1)).Returns(cat1Resources);
            mockAllReadyDataAccess.Setup(x => x.GetResourcesByCategory(resourceCat2)).Returns(cat2Resources);

            var controller = new ResourceApiController(mockAllReadyDataAccess.Object);

            //Act
            var cat1ResourceResults = controller.GetResourcesByCategory(resourceCat1);
            var cat2ResourceResults = controller.GetResourcesByCategory(resourceCat2);

            //Assert
            Assert.True(cat1ResourceResults.Any(x => x.Id == 1));
            Assert.True(cat2ResourceResults.Any(x => x.Id == 2));
        }
    }
}
