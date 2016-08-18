using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using System.Linq;
using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.Features.Requests;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddRequestsCommandHandlerAsync : IAsyncRequestHandler<AddRequestsCommand, bool>
    {
        private readonly IAllReadyDataAccess _data;
        private readonly IMediator _mediator;

        public AddRequestsCommandHandlerAsync( IAllReadyDataAccess data, IMediator mediator )
        {
            _data = data;
            _mediator = mediator;
        }

        public async Task<bool> Handle( AddRequestsCommand message )
        {
            Itinerary itinerary = await _data.GetItineraryByIdAsync(message.ItineraryId);

            if (itinerary == null)
            {
                // todo: sgordon: enhance this with a error message so the controller can better respond to the issue
                return false;
            }

            var requestsToUpdate = await _data.Requests
                .Where(r => message.RequestIdsToAdd.Contains(r.RequestId.ToString()))
                .ToList();

            HashSet<string> foundRequests = new HashSet<string>(requestsToUpdate.Select(s => s.RequestId.ToString()));

            var notFound = message.RequestIdsToAdd.Where(m => !foundRequests.Contains(m));

            if (notFound.Any())
            {
                // Something went wrong as some of the ids passed in where not matched in the database
                // todo: sgordon: we should enhance the returned object to include a message so that the controller can provide better feedback to the user
                return false;
            }

            if (requestsToUpdate.Count > 0)
            {
                var orderIndex = await _data.ItineraryRequests
                    .Where(i => i.ItineraryId == itinerary.Id)
                    .OrderByDescending(i => i.OrderIndex)
                    .Select(i => i.OrderIndex)
                    .FirstOrDefault();

                var itineraryRequestsToAdd = new List<ItineraryRequest>();

                foreach (var request in requestsToUpdate)
                {
                    orderIndex++;

                    if (request.Status == RequestStatus.Unassigned)
                    {
                        request.Status = RequestStatus.Assigned;

                        itineraryRequestsToAdd.Add(new ItineraryRequest
                        {
                            ItineraryId = itinerary.Id,
                            Request = request,
                            OrderIndex = orderIndex,
                            DateAssigned = DateTime.UtcNow // Note, we're storing system event dates as UTC time.
                        });


                        // todo: sgordon: Add a history record here and include the assigned date in the ItineraryRequest
                    }
                }

                await _data.AddItineraryRequests(itineraryRequestsToAdd);


                //On Successful addition of request
                Func<Request, Itinerary, string> getNotificationMessage = ( r, i ) => String.Format(ItinerariesMessages.RequestAddedInitialNotificationFormat, i.Date);

                await _mediator.SendAsync(new NotifyRequestorsCommand
                {
                    Requests = requestsToUpdate,
                    Itinerary = itinerary,
                    NotificationMessageBuilder = getNotificationMessage
                });
            }

            return true;
        }
    }
}