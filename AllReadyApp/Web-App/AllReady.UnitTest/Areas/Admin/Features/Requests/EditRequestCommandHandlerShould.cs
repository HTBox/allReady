using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Models;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using AllReady.Services.Mapping;
using AllReady.Services.Mapping.GeoCoding;
using AllReady.Services.Mapping.GeoCoding.Models;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    public class EditRequestCommandHandlerShould : InMemoryContextTest
    {
        private Request _existingRequest;
        protected override void LoadTestData()
        {
            var org = new Organization
            {
                Id = 1,
                Name = "testOrg",
            };

            Context.Organizations.Add(org);

            var campaign = new Campaign
            {
                ManagingOrganization = org
            };

            Context.Campaigns.Add(campaign);

            var @event = new Event
            {
                Id = 1,
                Campaign = campaign
            };

            Context.Events.Add(@event);

            _existingRequest = new Request
            {
                Address = "1234 Nowhereville",
                City = "Seattle",
                Name = "Request unit test",
                DateAdded = DateTime.MinValue,
                EventId = 1,
                Phone = "555-555-5555",
                Email = "something@example.com",
                State = "WA",
                PostalCode = "55555",
                Latitude = 10,
                Longitude = 10,
                Notes = "Some notes"
            };

            Context.Requests.Add(_existingRequest);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnNewRequestIdOnSuccessfulCreation()
        {
            var handler = new EditRequestCommandHandler(Context, new NullObjectGeocoder());
            var requestId = await handler.Handle(new EditRequestCommand { RequestModel = new EditRequestViewModel {  } });

            Assert.NotEqual(Guid.Empty, requestId);
        }

        [Fact]
        public async Task UpdateRequestsThatAlreadyExisted()
        {
            string expectedName = "replaced name";

            var handler = new EditRequestCommandHandler(Context, new NullObjectGeocoder());
            await handler.Handle(new EditRequestCommand
            {
                RequestModel = new EditRequestViewModel { Id = _existingRequest.RequestId, Name = expectedName }
            });

            var request = Context.Requests.First(r => r.RequestId == _existingRequest.RequestId);
            Assert.Equal(expectedName, request.Name );
        }

        [Fact]
        public async Task NotUpdateGeocodeIfAddressDidntChangeWhenUpdatingExistingRequest()
        {
            var mockGeocoder = new Mock<IGeocodeService>();

            var handler = new EditRequestCommandHandler(Context, mockGeocoder.Object);
            await handler.Handle(new EditRequestCommand
            {
                RequestModel = new EditRequestViewModel { Id = _existingRequest.RequestId, Address = _existingRequest.Address, City = _existingRequest.City, State = _existingRequest.State, PostalCode = _existingRequest.PostalCode }
            });

            mockGeocoder.Verify(x => x.GetCoordinatesFromAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task NotUpdateGeocodeWhenAddressChangedAndLatLonIsSetWhenUpdatingExistingRequest()
        {
            var mockGeocoder = new Mock<IGeocodeService>();
            string changedAddress = "4444 Changed Address Ln";

            var handler = new EditRequestCommandHandler(Context, mockGeocoder.Object);
            await handler.Handle(new EditRequestCommand
            {
                RequestModel = new EditRequestViewModel
                {
                    Id = _existingRequest.RequestId,
                    Address = changedAddress,
                    City = _existingRequest.City,
                    State = _existingRequest.State,
                    PostalCode = _existingRequest.PostalCode,
                    Notes = _existingRequest.Notes,
                    Latitude = 47.6,
                    Longitude = -122.3
                }
            });

            mockGeocoder.Verify(x => x.GetCoordinatesFromAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateGeocodeWhenAddressChangedAndLatLonNotSetWhenUpdatingExistingRequest()
        {
            var mockGeocoder = new Mock<IGeocodeService>();
            string changedAddress = "4444 Changed Address Ln";

            // Because the Geocode method takes a set of strings as arguments, actually verify the arguments are passed in to the mock in the correct order.
            mockGeocoder.Setup(g => g.GetCoordinatesFromAddress(changedAddress, _existingRequest.City, _existingRequest.State, _existingRequest.PostalCode, It.IsAny<string>())).ReturnsAsync(new Coordinates(0,0));

            var handler = new EditRequestCommandHandler(Context, mockGeocoder.Object);
            await handler.Handle(new EditRequestCommand
            {
                RequestModel = new EditRequestViewModel
                {
                    Id = _existingRequest.RequestId,
                    Address = changedAddress,
                    City = _existingRequest.City,
                    State = _existingRequest.State,
                    PostalCode = _existingRequest.PostalCode,
                    Latitude = 0,
                    Longitude = 0
                }
            });

            mockGeocoder.Verify(x => x.GetCoordinatesFromAddress(changedAddress, _existingRequest.City, _existingRequest.State, _existingRequest.PostalCode, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SetCorrectOrganizationId()
        {
            var handler = new EditRequestCommandHandler(Context, new NullObjectGeocoder());
            var requestId = await handler.Handle(new EditRequestCommand { RequestModel = new EditRequestViewModel { EventId = 1 } });

            var request = Context.Requests.FirstOrDefault(x => x.RequestId == requestId);

            request.OrganizationId.ShouldBe(1);
        }
    }
}
