using System;
using System.Threading.Tasks;
using AllReady.Features.Requests;
using AllReady.Models;
using Moq;
using Xunit;
using Geocoding;
using System.Collections.Generic;

namespace AllReady.UnitTest.Features.Requests
{
    public class AddRequestCommandHandlerAsyncTests : InMemoryContextTest
    {
        private AddRequestCommandHandlerAsync _sut;
        private Mock<IAllReadyDataAccess> _dataAccess;
        private Mock<IGeocoder> _geocoder;

        public AddRequestCommandHandlerAsyncTests()
        {
            _dataAccess = new Mock<IAllReadyDataAccess>();
            _geocoder = new Mock<IGeocoder>();
            _sut = new AddRequestCommandHandlerAsync(_dataAccess.Object, _geocoder.Object);
        }

        [Fact]
        public async Task HandleReturnsNullWhenNoErrorsOccur()
        {
            var command = new AddRequestCommandAsync
            {
                Request = new Request
                {
                    ProviderId = "successId"
                }
            };

            var result = await _sut.Handle(command);

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenNoProviderIdIsProvided_TheStatusIsUnassignedAndIdIsNotNull()
        {
            var request = new Request();
            var command = new AddRequestCommandAsync
            {
                Request = request
            };

            var result = await _sut.Handle(command);

            Assert.NotNull(request.RequestId);
            Assert.Equal(RequestStatus.Unassigned, request.Status);

        }

        [Fact]
        public async Task WhenProviderIdIsProvidedAndIsValid_TheStatusOfTheExistingRequestIsUpdated()
        {
            string pid = "someId";
            Guid rid = Guid.NewGuid();
            var request = new Request
            {
                ProviderId = pid,
                Status = RequestStatus.Assigned
            };
            var command = new AddRequestCommandAsync
            {
                Request = request
            };

            Request returnedRequest = new Request
            {
                ProviderId = pid,
                RequestId = rid,
                Status = RequestStatus.Unassigned
            };
            _dataAccess.Setup(x => x.GetRequestByProviderIdAsync(pid)).ReturnsAsync(returnedRequest);

            await _sut.Handle(command);

            Assert.Equal(RequestStatus.Assigned, returnedRequest.Status);
            _dataAccess.Verify(x => x.AddRequestAsync(returnedRequest));

        }

        [Fact]
        public async Task WhenLatitudeAndLogitudeAreNotProvided_SetsThemWithGeocodingAPI()
        {
            var request = new Request
            {
                ProviderId = "someId",
                Address = "1 Happy Street",
                City = "Happytown",
                State = "HP",
                Zip = "12345",
                Status = RequestStatus.Unassigned
            };

            var address = new Geocoding.Google.GoogleAddress(
                Geocoding.Google.GoogleAddressType.Premise,
                "formatted address",
                new[] { new Geocoding.Google.GoogleAddressComponent(
                    new [] { Geocoding.Google.GoogleAddressType.Country },
                    "",
                    "")},
                new Geocoding.Location(42, 24), //This is the only part that matters: the coordinates
                null,
                false,
                Geocoding.Google.GoogleLocationType.Rooftop);

            _geocoder.Setup(g => g.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty))
                .Returns(new[] { address });

            await _sut.Handle(new AddRequestCommandAsync { Request = request });

            Assert.Equal(42, request.Latitude);
            Assert.Equal(24, request.Longitude);
        }

        [Fact]
        public async Task WhenEitherLatitudeOrLogitudeAreProvided_DoNotChangeThem()
        {
            var request = new Request
            {
                ProviderId = "someId",
                Address = "1 Happy Street",
                City = "Happytown",
                State = "HP",
                Zip = "12345",
                Latitude = 13,
                Longitude = 14,
                Status = RequestStatus.Unassigned
            };

            var address = new Geocoding.Google.GoogleAddress(
                Geocoding.Google.GoogleAddressType.Premise,
                "formatted address",
                new[] { new Geocoding.Google.GoogleAddressComponent(
                    new [] { Geocoding.Google.GoogleAddressType.Country },
                    "",
                    "")},
                new Geocoding.Location(42, 24), //This is the only part that matters: the coordinates
                null,
                false,
                Geocoding.Google.GoogleLocationType.Rooftop);

            _geocoder.Setup(g => g.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty))
                .Returns(new[] { address });

            await _sut.Handle(new AddRequestCommandAsync { Request = request });

            Assert.Equal(13, request.Latitude);
            Assert.Equal(14, request.Longitude);

            //Clear latitude and re-test
            request.Latitude = 0;
            request.Longitude = 14;
            await _sut.Handle(new AddRequestCommandAsync { Request = request });

            Assert.Equal(0, request.Latitude);
            Assert.Equal(14, request.Longitude);

            //Clear longitude and re-test
            request.Latitude = 13;
            request.Longitude = 0;
            await _sut.Handle(new AddRequestCommandAsync { Request = request });

            Assert.Equal(13, request.Latitude);
            Assert.Equal(0, request.Longitude);
        }
    }
}