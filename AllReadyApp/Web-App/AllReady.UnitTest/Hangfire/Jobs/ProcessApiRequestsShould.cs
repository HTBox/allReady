using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Requests;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using AllReady.Services.Mapping;
using AllReady.Services.Mapping.GeoCoding;
using AllReady.Services.Mapping.GeoCoding.Models;
using AllReady.ViewModels.Requests;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
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

            var sut = new ProcessApiRequests(Context, Mock.Of<IMediator>(), Mock.Of<IGeocodeService>(), Options.Create(new ApprovedRegionsSettings()))
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

        [Fact]
        public void AssignCorrectValuesToRequestsLatitiudeAndLongitudeWhenIGeocoderReturnedAdressIsNull()
        {
            var requestId = Guid.NewGuid();
            var geocoder = new Mock<IGeocodeService>();
            geocoder.Setup(service => service.GetCoordinatesFromAddress(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult<Coordinates>(null));

            var sut = new ProcessApiRequests(Context, Mock.Of<IMediator>(), geocoder.Object,
                Options.Create(new ApprovedRegionsSettings()))
            {
                NewRequestId = () => requestId
            };

            sut.Process(new RequestApiViewModel());

            var request = Context.Requests.Single(r => r.RequestId == requestId);

            request.Latitude.ShouldBe(0);
            request.Longitude.ShouldBe(0);
        }

        [Fact]
        public void AssignCorrectValuesToRequestsLatitiudeAndLongitudeWhenIGeocoderReturnedAdressIsNotNull()
        {
            var requestId = Guid.NewGuid();
            var latitude = 20.013;
            var longitude = 40.058;

            var geocoder = new Mock<IGeocodeService>();
            geocoder.Setup(service => service.GetCoordinatesFromAddress(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(new Coordinates(latitude, longitude)));

            var sut = new ProcessApiRequests(Context, Mock.Of<IMediator>(), geocoder.Object,
                Options.Create(new ApprovedRegionsSettings()))
            {
                NewRequestId = () => requestId
            };

            sut.Process(new RequestApiViewModel());

            var request = Context.Requests.Single(r => r.RequestId == requestId);

            request.Latitude.ShouldBe(latitude);
            request.Longitude.ShouldBe(longitude);
        }

        [Fact]
        public void InvokeIGeocoderWithTheCorrectParameters()
        {
            var requestId = Guid.NewGuid();
            var geoCoder = new Mock<IGeocodeService>();
            var viewModel = new RequestApiViewModel { Address = "address", City = "city", State = "state", Zip = "zip" };
            var sut = new ProcessApiRequests(Context, Mock.Of<IMediator>(), geoCoder.Object, Options.Create(new ApprovedRegionsSettings()))
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

            var sut = new ProcessApiRequests(Context, mediator.Object, Mock.Of<IGeocodeService>(), Options.Create(new ApprovedRegionsSettings()))
            {
                NewRequestId = () => requestId
            };

            sut.Process(new RequestApiViewModel());

            mediator.Verify(x => x.Publish(It.Is<ApiRequestProcessedNotification>(y => y.RequestId == requestId)), Times.Once);
        }

        [Fact]
        public void PublishApiRequestAddedNotificationWithTrueAcceptanceApprovedRegionsAreDisabled()
        {
            var mediator = new Mock<IMediator>();

            var approvedRegions = Options.Create(new ApprovedRegionsSettings
            {
                Enabled = false
            });

            var sut = new ProcessApiRequests(Context, mediator.Object, Mock.Of<IGeocodeService>(), approvedRegions)
            {
                NewRequestId = () => Guid.NewGuid()
            };

            sut.Process(new RequestApiViewModel
            {
                ProviderData = "approved_region"
            });

            mediator.Verify(x => x.Publish(It.Is<ApiRequestProcessedNotification>(y => y.Acceptance == true)), Times.Once);
        }

        [Fact]
        public void PublishApiRequestAddedNotificationWithTrueAcceptanceWhenInsideApprovedRegions()
        {
            var mediator = new Mock<IMediator>();

            var approvedRegions = Options.Create(new ApprovedRegionsSettings
            {
                Enabled = true,
                Regions = new List<string>
                {
                    "approved_region"
                }
            });

            var sut = new ProcessApiRequests(Context, mediator.Object, Mock.Of<IGeocodeService>(), approvedRegions)
            {
                NewRequestId = () => Guid.NewGuid()
            };

            sut.Process(new RequestApiViewModel
            {
                ProviderData = "approved_region"
            });

            mediator.Verify(x => x.Publish(It.Is<ApiRequestProcessedNotification>(y => y.Acceptance == true)), Times.Once);
        }

        [Fact]
        public void PublishApiRequestAddedNotificationWithFalseAcceptanceWhenOutsideApprovedRegions()
        {
            var mediator = new Mock<IMediator>();

            var approvedRegions = Options.Create(new ApprovedRegionsSettings
            {
                Enabled = true,
                Regions = new List<string>
                {
                    "approved_region"
                }
            });

            var sut = new ProcessApiRequests(Context, mediator.Object, Mock.Of<IGeocodeService>(), approvedRegions)
            {
                NewRequestId = () => Guid.NewGuid()
            };

            sut.Process(new RequestApiViewModel
            {
                ProviderData = "non_approved_region"
            });

            mediator.Verify(x => x.Publish(It.Is<ApiRequestProcessedNotification>(y => y.Acceptance == false)), Times.Once);
        }
    }
}