using AllReady.Features.Resource;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Resource
{
    public class ResourcesByCategoryQueryHandlerTests
    {
        [Fact]
        public void HandleInvokesGetResourcesByCategoryWithCorrectCategory()
        {
            var message = new ResourcesByCategoryQuery { Category = "category" };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new ResourcesByCategoryQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetResourcesByCategory(message.Category));
        }
    }
}
