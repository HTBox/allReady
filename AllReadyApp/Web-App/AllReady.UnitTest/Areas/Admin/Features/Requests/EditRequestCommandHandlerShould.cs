using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Models;
using AllReady.Providers;
using MediatR;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    public class EditRequestCommandHandlerShould : InMemoryContextTest
    {
        private Request _existingRequest;
        protected override void LoadTestData()
        {
            _existingRequest = new Request
            {
                Address = "1234 Nowhereville",
                City = "",
                Name = "",
                DateAdded = DateTime.MinValue,
                EventId = 1,
                Phone = "555-555-5555",
                Email = "something@example.com",
                State = "WA",
                Zip = "55555"
            };

            Context.Requests.Add(_existingRequest);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnNewRequestIdOnSuccessfulCreation()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new EditRequestCommandHandler(Context, new NullObjectGeocoder());
            var requestId = await handler.Handle(new EditRequestCommand { RequestModel = new EditRequestViewModel {  } });

            Assert.NotEqual(Guid.Empty, requestId);
        }

        [Fact]
        public async Task UpdateRequestsThatAlreadyExisted()
        {
            var mockMediator = new Mock<IMediator>();
            string expectedName = "replaced name";

            var handler = new EditRequestCommandHandler(Context, new NullObjectGeocoder());
            var requestId = await handler.Handle(new EditRequestCommand
            {
                RequestModel = new EditRequestViewModel { Id = _existingRequest.RequestId, Name = expectedName }
            });

            var request = Context.Requests.First(r => r.RequestId == _existingRequest.RequestId);
            Assert.Equal(expectedName, request.Name );
        }
    }
}
