using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Configuration;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using AllReady.Services.Mapping.GeoCoding;
using AllReady.Services.Mapping.GeoCoding.Models;
using AllReady.ViewModels.Requests;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Hangfire.Jobs
{
    public class ProcessApiRequestsShould : InMemoryContextTest
    {
        [Fact]
        public void NotSaveRequestNotInvokeIGeocodeServiceAndNotNotEnqueueISendRequestStatusToGetASmokeAlarm_WhenRequestExists()
        {
            const string providerRequestId = "1";
            var model = new RequestApiViewModel { ProviderRequestId = providerRequestId };

            Context.Requests.Add(new Request { ProviderRequestId = providerRequestId });
            Context.SaveChanges();

            var geocodeService = new Mock<IGeocodeService>();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            var sut = new ProcessApiRequests(Context, geocodeService.Object, Mock.Of<IOptions<ApprovedRegionsSettings>>(), backgroundJobClient.Object);
            sut.Process(model);

            Assert.Equal(1, Context.Requests.Count(x => x.ProviderRequestId == providerRequestId));
            geocodeService.Verify(x => x.GetCoordinatesFromAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == nameof(ISendRequestStatusToGetASmokeAlarm.Send)), It.IsAny<EnqueuedState>()), Times.Never);
        }

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
                PostalCode = postalCode,
                Phone = "111-111-1111",
                Email = "email@email.com",
                ProviderData = "ProviderData"
            };

            var sut = new ProcessApiRequests(Context, Mock.Of<IGeocodeService>(), Options.Create(new ApprovedRegionsSettings()), Mock.Of<IBackgroundJobClient>())
            {
                NewRequestId = () => requestId,
                DateTimeUtcNow = () => dateAdded
            };
            sut.Process(viewModel);

            var request = Context.Requests.Single(x => x.RequestId == requestId);

            Assert.Equal(request.RequestId, requestId);
            Assert.Equal(1, request.OrganizationId);
            Assert.Equal(request.ProviderRequestId, viewModel.ProviderRequestId);
            Assert.Equal(request.ProviderData, viewModel.ProviderData);
            Assert.Equal(request.Address, viewModel.Address);
            Assert.Equal(request.City, viewModel.City);
            Assert.Equal(request.DateAdded, dateAdded);
            Assert.Equal(request.Email, viewModel.Email);
            Assert.Equal(request.Name, viewModel.Name);
            Assert.Equal(request.State, viewModel.State);
            Assert.Equal(request.PostalCode, viewModel.PostalCode);
            Assert.Equal(RequestStatus.Unassigned, request.Status);
            Assert.Equal(RequestSource.Api, request.Source);
        }

        [Fact]
        public void InvokeIGeocoderWithTheCorrectParameters()
        {
            var requestId = Guid.NewGuid();
            var geoCoder = new Mock<IGeocodeService>();
            var viewModel = new RequestApiViewModel { Address = "address", City = "city", State = "state", PostalCode = "postcode" };
            var sut = new ProcessApiRequests(Context, geoCoder.Object, Options.Create(new ApprovedRegionsSettings()), Mock.Of<IBackgroundJobClient>())
            {
                NewRequestId = () => requestId
            };

            sut.Process(viewModel);

            geoCoder.Verify(x => x.GetCoordinatesFromAddress(viewModel.Address, viewModel.City, viewModel.State, viewModel.PostalCode, string.Empty), Times.Once);
        }

        [Fact]
        public void AssignCorrectValuesToRequestsLatitiudeAndLongitudeWhenIGeocoderReturnedAdressIsNull()
        {
            var requestId = Guid.NewGuid();
            var geocoder = new Mock<IGeocodeService>();
            geocoder.Setup(service => service.GetCoordinatesFromAddress(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Coordinates)null);

            var sut = new ProcessApiRequests(Context, geocoder.Object,
                Options.Create(new ApprovedRegionsSettings()), Mock.Of<IBackgroundJobClient>())
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
            const double latitude = 20.013;
            const double longitude = 40.058;

            var geocoder = new Mock<IGeocodeService>();
            geocoder.Setup(service => service.GetCoordinatesFromAddress(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Coordinates(latitude, longitude));

            var sut = new ProcessApiRequests(Context, geocoder.Object,
                Options.Create(new ApprovedRegionsSettings()), Mock.Of<IBackgroundJobClient>())
            {
                NewRequestId = () => requestId
            };

            sut.Process(new RequestApiViewModel());

            var request = Context.Requests.Single(r => r.RequestId == requestId);

            request.Latitude.ShouldBe(latitude);
            request.Longitude.ShouldBe(longitude);
        }

        [Fact]
        public void InvokeISendRequestStatusToGetASmokeAlarmWithTheCorrectProviderRequestIdAndStatus()
        {
            var viewModel = new RequestApiViewModel { ProviderRequestId = "1" };
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            var sut = new ProcessApiRequests(Context, Mock.Of<IGeocodeService>(), 
                Options.Create(new ApprovedRegionsSettings()), backgroundJobClient.Object);

            sut.Process(viewModel);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == nameof(ISendRequestStatusToGetASmokeAlarm.Send) && 
                job.Args[0].ToString() == viewModel.ProviderRequestId &&
                job.Args[1].ToString() == GasaStatus.New), It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void InvokeISendRequestStatusToGetASmokeAlarmWithAnAcceptanceOfTrue_WhenApprovedRegionsAreDisabled()
        {
            var viewModel = new RequestApiViewModel { ProviderRequestId = "1" };
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            var sut = new ProcessApiRequests(Context, Mock.Of<IGeocodeService>(),
                Options.Create(new ApprovedRegionsSettings()), backgroundJobClient.Object);

            sut.Process(viewModel);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => (bool)job.Args[2]), It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void InvokeISendRequestStatusToGetASmokeAlarmWithAnAcceptanceOfTrue_WhenApprovedRegionsAreEnabledAndRegionIsApproved()
        {
            const string providerData = "region";

            var viewModel = new RequestApiViewModel { ProviderRequestId = "1", ProviderData = providerData };
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            var sut = new ProcessApiRequests(Context, Mock.Of<IGeocodeService>(),
                Options.Create(new ApprovedRegionsSettings { Enabled = true, Regions = new List<string> { providerData } }), backgroundJobClient.Object);

            sut.Process(viewModel);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => (bool)job.Args[2]), It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void InvokeISendRequestStatusToGetASmokeAlarmWithAnAcceptanceOfFalse_WhenApprovedRegionsAreEnabledAndRegionIsNotApproved()
        {
            var viewModel = new RequestApiViewModel { ProviderRequestId = "1", ProviderData = "region" };
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            var sut = new ProcessApiRequests(Context, Mock.Of<IGeocodeService>(),
                Options.Create(new ApprovedRegionsSettings { Enabled = true, Regions = new List<string> { "UnapprovedRegion" } }), backgroundJobClient.Object);

            sut.Process(viewModel);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => (bool)job.Args[2] == false), It.IsAny<EnqueuedState>()));
        }
    }
}
