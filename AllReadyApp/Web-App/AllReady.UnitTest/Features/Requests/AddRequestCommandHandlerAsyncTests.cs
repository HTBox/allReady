using System;
using System.Threading.Tasks;
using AllReady.Features.Requests;
using AllReady.Models;
using Moq;
using Xunit;
using Geocoding;
using System.Collections.Generic;
using System.Linq;

namespace AllReady.UnitTest.Features.Requests
{
    public class AddRequestCommandHandlerAsyncTests : InMemoryContextTest
    {

        [Fact]
        public async Task HandleReturnsNullWhenNoErrorsOccur() {

            var command = new AddRequestCommandAsync {
                Request = new Request {ProviderId = "successId"}
            };

            var geocoder = new Mock<IGeocoder>();
            AddRequestCommandHandlerAsync sut = new AddRequestCommandHandlerAsync(this.Context, geocoder.Object);

            var result = await sut.Handle(command);

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenNoProviderIdIsProvided_TheStatusIsUnassignedAndIdIsNotNull()
        {
            var request = new Request();
            var command = new AddRequestCommandAsync {Request = request};

            var geocoder = new Mock<IGeocoder>();
            AddRequestCommandHandlerAsync sut = new AddRequestCommandHandlerAsync(this.Context, geocoder.Object);
            var result = await sut.Handle(command);

            Assert.NotNull(request.RequestId);
            Assert.Equal(RequestStatus.Unassigned, request.Status);

        }

        [Fact]
        public async Task WhenProviderIdIsProvidedAndIsValid_TheStatusOfTheExistingRequestIsUpdated()
        {
            string pid = "someId";
            var request = new Request
            {
                ProviderId = pid,
                Status = RequestStatus.Assigned
            };
            var command = new AddRequestCommandAsync{Request = request};

            var options = this.CreateNewContextOptions();

            using (var context = new AllReadyContext(options)) {
                context.Requests.Add(new Request {
                    ProviderId = pid,
                    Status = RequestStatus.Assigned
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var geocoder = new Mock<IGeocoder>();
                AddRequestCommandHandlerAsync sut = new AddRequestCommandHandlerAsync(context, geocoder.Object);


                await sut.Handle(command);
            }

            using (var context = new AllReadyContext(options)) {
                var entity = context.Requests.FirstOrDefault(x => x.ProviderId == pid);
                Assert.Equal(entity.Status, RequestStatus.Assigned);
            }
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

            var geocoder = new Mock<IGeocoder>();
            AddRequestCommandHandlerAsync sut = new AddRequestCommandHandlerAsync(this.Context, geocoder.Object);

            geocoder.Setup(g => g.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty))
                .Returns(new[] { address });

            await sut.Handle(new AddRequestCommandAsync { Request = request });

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

            var geocoder = new Mock<IGeocoder>();
            AddRequestCommandHandlerAsync sut = new AddRequestCommandHandlerAsync(this.Context, geocoder.Object);

            geocoder.Setup(g => g.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty))
                .Returns(new[] { address });

            await sut.Handle(new AddRequestCommandAsync { Request = request });

            Assert.Equal(13, request.Latitude);
            Assert.Equal(14, request.Longitude);

            //Clear latitude and re-test
            request.Latitude = 0;
            request.Longitude = 14;
            await sut.Handle(new AddRequestCommandAsync { Request = request });

            Assert.Equal(0, request.Latitude);
            Assert.Equal(14, request.Longitude);

            //Clear longitude and re-test
            request.Latitude = 13;
            request.Longitude = 0;
            await sut.Handle(new AddRequestCommandAsync { Request = request });

            Assert.Equal(13, request.Latitude);
            Assert.Equal(0, request.Longitude);
        }
    }
}
