using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddRequestsCommandHandlerAsync : IAsyncRequestHandler<AddRequestsCommand, bool>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public AddRequestsCommandHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AddRequestsCommand message)
        {
            var itinerary = await _context.Itineraries
              .Where(x => x.Id == message.ItineraryId)
              .Select(x => new
              {
                  Id = x.Id,
                  Name = x.Name
              }).SingleOrDefaultAsync();

            if (itinerary == null)
            {
                // todo: sgordon: enhance this with a error message so the controller can better respond to the issue
                return false;
            }

            var requestsToUpdate = await _context.Requests
                .Where(r => message.RequestIdsToAdd.Contains(r.RequestId.ToString()))
                .ToListAsync();

            HashSet<string> foundRequests = new HashSet<string>(requestsToUpdate.Select(s => s.RequestId.ToString()));

            var notFound = message.RequestIdsToAdd.Where(m => !foundRequests.Contains(m));

            if (notFound.Count() > 0)
            {
                // Something went wrong as some of the ids passed in where not matched in the database
                // todo: sgordon: we should enhance the returned object to include a message so that the controller can provide better feedback to the user
                return false;
            }

            if (requestsToUpdate.Count > 0)
            {
                var orderIndex = await _context.ItineraryRequests.AsNoTracking()
                    .Where(i => i.ItineraryId == itinerary.Id)
                    .OrderByDescending(i => i.OrderIndex)
                    .Select(i => i.OrderIndex)
                    .FirstOrDefaultAsync();

                foreach (var request in requestsToUpdate)
                {
                    orderIndex++;

                    if (request.Status == RequestStatus.UnAssigned)
                    {
                        request.Status = RequestStatus.Assigned;

                        _context.ItineraryRequests.Add(new ItineraryRequest
                        {
                            ItineraryId = itinerary.Id,
                            Request = request,
                            OrderIndex = orderIndex,
                            DateAssigned = DateTime.UtcNow // Note, we're storing system event dates as UTC time.
                        });

                        // todo: sgordon: Add a history record here and include the assigned date in the ItineraryRequest
                    }
                }

                await _context.SaveChangesAsync();
            }        

            return true;
        }
    }
}