using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestListItemsQueryHandlerAsync : IAsyncRequestHandler<RequestListItemsQuery, List<RequestListModel>>
    {
        private readonly AllReadyContext _context;

        public RequestListItemsQueryHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
        }

        public async Task<List<RequestListModel>> Handle(RequestListItemsQuery message)
        {
            var results = _context.Requests.AsNoTracking()
                .Where(r => r.Status == RequestStatus.UnAssigned);

            // Apply filtering based on criteria
            if (message.criteria.RequestId.HasValue)
            { 
                results = results.Where(r => r.RequestId == message.criteria.RequestId.Value);
            }

            if (message.criteria.IncludeAssigned)
            {
                results = results.Where(r => r.Status == RequestStatus.Assigned);
            }

            if (message.criteria.IncludeCanceled)
            {
                results = results.Where(r => r.Status == RequestStatus.Canceled);
            }

            if (message.criteria.EventId.HasValue)
            {
                results = results.Where(r => r.EventId == message.criteria.EventId.Value);
            }

            // todo: sgordon: date added filtering

            return await results.Select(r => new RequestListModel
            {
                Id = r.RequestId,
                Name = r.Name,
                Address = r.Address,
                City = r.City,
                Status = r.Status,
                DateAdded = r.DateAdded
            }).ToListAsync().ConfigureAwait(false);
        }
    }
}
