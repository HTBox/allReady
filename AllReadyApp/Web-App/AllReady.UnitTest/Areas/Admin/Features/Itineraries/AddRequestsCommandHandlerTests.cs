using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;
using AddRequestsCommand = AllReady.Areas.Admin.Features.Itineraries.AddRequestsCommand;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class AddRequestsCommandHandlerTests
    {
    //    private readonly Mock<IMediator> _mediator;
    //    private AddRequestsCommandHandlerAsync _sut;
    //    private readonly Mock<IAllReadyDataAccess> _data;

    //    public AddRequestsCommandHandlerTests()
    //    {
    //        _data = new Mock<IAllReadyDataAccess>();
    //        _mediator = new Mock<IMediator>();
    //        _sut = new AddRequestsCommandHandlerAsync(_data.Object, _mediator.Object);
    //    }

    //    [Fact]
    //    public async Task WhenAddingRequestsSucceeds_InitialNotificationCommandIsSent()
    //    {
    //        var id = 42;
    //        var itinerary = new Itinerary
    //        {
    //            Id = id
    //        };

    //        var requests = new List<Request>
    //        {
    //            new Request
    //            {
    //                RequestId = Guid.NewGuid()
    //            },
    //            new Request
    //            {
    //                RequestId = Guid.NewGuid()
    //            },
    //            new Request
    //            {
    //                RequestId = Guid.NewGuid()
    //            }
    //        };

    //        var command = new AddRequestsCommand
    //        {
    //            ItineraryId = itinerary.Id,
    //            RequestIdsToAdd = requests.Select(x => x.RequestId.ToString()).ToList()
    //        };

    //        var iRequestList = new List<ItineraryRequest>();

    //        foreach (var request in requests)
    //        {
    //            iRequestList.Add(new ItineraryRequest
    //            {
    //                Itinerary = itinerary,
    //                ItineraryId = id,
    //                OrderIndex = 1,
    //                Request = request,
    //                RequestId = request.RequestId
    //            });
    //        }

    //        _data.Setup(x => x.GetItineraryByIdAsync(id)).ReturnsAsync(itinerary);
    //        _data.SetupGet(x => x.Requests).Returns(requests.ToAsyncEnumerable());
    //        _data.SetupGet(x => x.ItineraryRequests).Returns(iRequestList.ToAsyncEnumerable());

    //        await _sut.Handle(command);

    //        _mediator.Verify(x => x.SendAsync(It.Is<NotifyRequestorsCommand>(
    //            c => c.Itinerary.Id == itinerary.Id)));
    //    }
    }
}