using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Requests;
using AllReady.Models;
using AllReady.ViewModels.Requests;
using Geocoding;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class AddApiRequestCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task AddRequest()
        {
            var requestId = Guid.NewGuid();
            var dateAdded = DateTime.UtcNow;

            var viewModel = new RequestApiViewModel
            {
                ProviderRequestId = "ProviderRequestId",
                Name = "Name",
                Address = "Address",
                City = "City",
                State = "state",
                Zip = "zip",
                Phone = "phone",
                Email = "email",
                Status = "new",
                ProviderData = "ProviderData"
            };
            
            var message = new AddApiRequestCommand { ViewModel = viewModel };

            var sut = new AddApiRequestCommandHandler(Context, Mock.Of<IGeocoder>(), Mock.Of<IMediator>())
            {
                NewRequestId = () => requestId,
                DateTimeUtcNow = () => dateAdded
            };
            await sut.Handle(message);

            var request = Context.Requests.Single(x => x.RequestId == requestId);

            Assert.Equal(request.RequestId, requestId);
            Assert.Equal(request.ProviderRequestId, viewModel.ProviderRequestId);
            Assert.Equal(request.ProviderData, viewModel.ProviderData);
            Assert.Equal(request.Address, viewModel.Address);
            Assert.Equal(request.City, viewModel.City);
            Assert.Equal(request.DateAdded, dateAdded);
            Assert.Equal(request.Email, viewModel.Email);
            Assert.Equal(request.Name, viewModel.Name);
            Assert.Equal(request.State, viewModel.State);
            Assert.Equal(request.Zip, viewModel.Zip);
            Assert.Equal(request.Status, RequestStatus.Unassigned);
            Assert.Equal(request.Source, RequestSource.Api);
        }

        [Fact]
        public async Task PublishApiRequestAddedNotificationWithTheCorrectRequestId()
        {
            var requestId = Guid.NewGuid();
            var message = new AddApiRequestCommand { ViewModel = new RequestApiViewModel() };

            var mediator = new Mock<IMediator>();

            var sut = new AddApiRequestCommandHandler(Context, Mock.Of<IGeocoder>(), mediator.Object)
            {
                NewRequestId = () => requestId
            };

            await sut.Handle(message);

            mediator.Verify(x => x.PublishAsync(It.Is<ApiRequestAddedNotification>(y  => y.RequestId == requestId)), Times.Once);
        }
    }
}