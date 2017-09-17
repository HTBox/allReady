using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using System.Linq;
using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.Features.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddRequestsToItineraryCommandHandler : IAsyncRequestHandler<AddRequestsToItineraryCommand, bool>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public AddRequestsToItineraryCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AddRequestsToItineraryCommand message)
        {
            var itinerary = await _context.Itineraries
               .Where(x => x.Id == message.ItineraryId)
               .Select(x => new { x.Id, x.Name})
               .SingleOrDefaultAsync();

            if (itinerary == null)
            {
                // todo: sgordon: enhance this with a error message so the controller can better respond to the issue
                return false;
            }

            var requestsToUpdate = await _context.Requests.AsAsyncEnumerable()
                .Where(r => message.RequestIdsToAdd.Contains(r.RequestId.ToString()))
                .ToList();
            
            var foundRequests = new HashSet<string>(requestsToUpdate.Select(s => s.RequestId.ToString()));

            var notFound = message.RequestIdsToAdd.Where(m => !foundRequests.Contains(m));

            if (notFound.Any())
            {
                // Something went wrong as some of the ids passed in where not matched in the database
                // todo: sgordon: we should enhance the returned object to include a message so that the controller can provide better feedback to the user
                return false;
            }

            if (requestsToUpdate.Count > 0)
            {
                var orderIndex = await _context.ItineraryRequests.AsAsyncEnumerable()
                    .Where(i => i.ItineraryId == itinerary.Id)
                    .OrderByDescending(i => i.OrderIndex)
                    .Select(i => i.OrderIndex)
                    .FirstOrDefault();

                foreach (var request in requestsToUpdate)
                {
                    orderIndex++;

                    if (request.Status == RequestStatus.Unassigned)
                    {
                        request.Status = RequestStatus.Assigned;

                        _context.ItineraryRequests.Add(new ItineraryRequest
                        {
                            ItineraryId = itinerary.Id,
                            Request = request,
                            OrderIndex = orderIndex,
                            DateAssigned = DateTimeUtcNow()
                        });
                    }
                }

                await _context.SaveChangesAsync();

                await _mediator.PublishAsync(new RequestsAssignedToItinerary { ItineraryId = message.ItineraryId, RequestIds = requestsToUpdate.Select(x => x.RequestId).ToList() });
            }
            return true;
        }
    }
}
