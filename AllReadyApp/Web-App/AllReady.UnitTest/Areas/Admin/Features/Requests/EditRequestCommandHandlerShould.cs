using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Providers;
using MediatR;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    public class EditRequestCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
        }

        [Fact]
        public async Task ReturnNewRequestIdOnSuccessfulCreation()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new EditRequestCommandHandler(Context, new NullObjectGeocoder());
            var requestId = await handler.Handle(new EditRequestCommand { RequestModel = new EditRequestViewModel {  } });

            Assert.NotEqual(Guid.Empty, requestId);
        }
    }
}
