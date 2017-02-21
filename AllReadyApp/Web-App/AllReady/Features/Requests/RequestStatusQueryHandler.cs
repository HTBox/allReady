using AllReady.Models;
using AllReady.ViewModels.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Features.Requests
{

    public class RequestStatusQuery : IAsyncRequest<RequestStatusViewModel>
    {
        public Guid RequestId { get; set; }
    }

    public class RequestStatusQueryHandler : IAsyncRequestHandler<RequestStatusQuery, RequestStatusViewModel>
    {
        private readonly AllReadyContext context;

        public RequestStatusQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public async Task<RequestStatusViewModel> Handle(RequestStatusQuery message)
        {
            RequestStatusViewModel result = null;

            var request = await context.Requests.FirstOrDefaultAsync(x => x.RequestId == message.RequestId);
            
            if (request != null)
            {
                var itinReq = await context.ItineraryRequests.Include(x => x.Itinerary).FirstOrDefaultAsync(x => x.RequestId == message.RequestId);

                result = new RequestStatusViewModel {
                    Status = request.Status,
                    HasItineraryItems = itinReq != null                   
                };

                if (result.HasItineraryItems)
                    result.PlannedDeploymentDate = itinReq.Itinerary.Date;
            }

            return result;
        }
    }
    
}
