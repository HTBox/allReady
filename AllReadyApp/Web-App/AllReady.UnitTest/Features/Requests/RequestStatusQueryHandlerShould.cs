using AllReady.Features.Requests;
using AllReady.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class RequestStatusQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task RequestDoesNotExist_ReturnsNull()
        {
            //Arrange
            Guid requestId = new Guid("7d0ee546-39a2-41fc-b8f2-d54c7f9ef234");

            var request = new Request { RequestId = requestId };
            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new RequestStatusQueryHandler(Context);
            var message = new RequestStatusQuery { RequestId = new Guid() };
            //Act
            var result = await sut.Handle(message);
            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task HasItineraryItems_ReturnsTrueAndDisplaysCorrectPublishedDate()
        {
            //Arrange
            Guid requestId = Guid.NewGuid();
            DateTime itineraryDate = new DateTime(1);
            const int itineraryId = 1;

            var request = new Request { RequestId = requestId };
            Context.Requests.Add(request);

            var itinerary = new Itinerary { Id = itineraryId, Date = itineraryDate };
            Context.Itineraries.Add(itinerary);

            var itineraryRequest = new ItineraryRequest { RequestId = requestId, ItineraryId = itineraryId };
            Context.ItineraryRequests.Add(itineraryRequest);
            Context.SaveChanges();

            var sut = new RequestStatusQueryHandler(Context);
            var message = new RequestStatusQuery { RequestId = requestId };
            //Act
            var result = await sut.Handle(message);
            //Assert
            Assert.True(result.HasItineraryItems);
            Assert.Equal(result.PlannedDeploymentDate, itineraryDate);
        }



    }
}
