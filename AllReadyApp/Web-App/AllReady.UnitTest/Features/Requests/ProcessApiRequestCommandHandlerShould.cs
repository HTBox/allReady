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
    public class ProcessApiRequestCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task AddRequest()
        {
            var requestId = Guid.NewGuid();
            var dateAdded = DateTime.UtcNow;
            const string postalCode = "11111";

            var viewModel = new RequestApiViewModel
            {
                ProviderRequestId = "ProviderRequestId",
                Status = "new",
                Name = "Name",
                Address = "Address",
                City = "City",
                State = "state",
                Zip = postalCode,
                Phone = "111-111-1111",
                Email = "email@email.com",
                ProviderData = "ProviderData"
            };

            var message = new ProcessApiRequestCommand { ViewModel = viewModel };

            var sut = new ProcessApiRequestCommandHandler(Context, Mock.Of<IGeocoder>(), Mock.Of<IMediator>())
            {
                NewRequestId = () => requestId,
                DateTimeUtcNow = () => dateAdded
            };
            await sut.Handle(message);

            var request = Context.Requests.Single(x => x.RequestId == requestId);

            Assert.Equal(request.RequestId, requestId);
            Assert.Equal(request.ProviderId, viewModel.ProviderRequestId);
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
        public async Task PublishApiRequestAddedNotificationWithTheCorrectProviderRequestId()
        {
            const string providerRequestId = "ProviderRequestId";
            var requestId = Guid.NewGuid();
            var message = new ProcessApiRequestCommand { ViewModel = new RequestApiViewModel { ProviderRequestId = providerRequestId }};

            var mediator = new Mock<IMediator>();

            var sut = new ProcessApiRequestCommandHandler(Context, Mock.Of<IGeocoder>(), mediator.Object)
            {
                NewRequestId = () => requestId,
            };

            await sut.Handle(message);

            mediator.Verify(x => x.PublishAsync(It.Is<ApiRequestProcessedNotification>(y  => y.ProviderRequestId == providerRequestId)), Times.Once);
        }
    }
}