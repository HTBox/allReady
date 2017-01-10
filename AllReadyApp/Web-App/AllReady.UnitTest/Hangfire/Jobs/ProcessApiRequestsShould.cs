using System;
using System.Linq;
using AllReady.Features.Requests;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using AllReady.Services.Mapping;
using AllReady.ViewModels.Requests;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Hangfire.Jobs
{
    public class ProcessApiRequestsShould : InMemoryContextTest
    {
        [Fact]
        public void AddRequest()
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

            var sut = new ProcessApiRequests(Context, Mock.Of<IMediator>(), Mock.Of<IGeocodeService>())
            {
                NewRequestId = () => requestId,
                DateTimeUtcNow = () => dateAdded
            };
            sut.Process(viewModel);

            var request = Context.Requests.Single(x => x.RequestId == requestId);

            Assert.Equal(request.RequestId, requestId);
            Assert.Equal(request.OrganizationId, 1);
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

        [Fact(Skip = "NotImplemented")]
        public void AssignCorrectValuesToRequestsLatitiudeAndLongitudeWhenIGeocoderReturnedAdressIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignCorrectValuesToRequestsLatitiudeAndLongitudeWhenIGeocoderReturnedAdressIsNotNull()
        {
        }

        [Fact]
        public void InvokeIGeocoderWithTheCorrectParameters()
        {
            var requestId = Guid.NewGuid();
            var geoCoder = new Mock<IGeocodeService>();
            var viewModel = new RequestApiViewModel { Address = "address", City = "city", State = "state", Zip = "zip" };
            var sut = new ProcessApiRequests(Context, Mock.Of<IMediator>(), geoCoder.Object)
            {
                NewRequestId = () => requestId
            };

            sut.Process(viewModel);

            geoCoder.Verify(x => x.GetCoordinatesFromAddress(viewModel.Address, viewModel.City, viewModel.State, viewModel.Zip, string.Empty), Times.Once);
        }

        [Fact]
        public void PublishApiRequestAddedNotificationWithTheCorrectRequestId()
        {
            var requestId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();

            var sut = new ProcessApiRequests(Context, mediator.Object, Mock.Of<IGeocodeService>())
            {
                NewRequestId = () => requestId
            };

            sut.Process(new RequestApiViewModel());

            mediator.Verify(x => x.Publish(It.Is<ApiRequestProcessedNotification>(y => y.RequestId == requestId)), Times.Once);
        }
    }
}