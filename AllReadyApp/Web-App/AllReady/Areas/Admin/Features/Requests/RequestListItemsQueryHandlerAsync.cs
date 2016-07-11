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
            if (message.Criteria.RequestId.HasValue)
            { 
                results = results.Where(r => r.RequestId == message.Criteria.RequestId.Value);
            }

            if (message.Criteria.IncludeAssigned)
            {
                results = results.Where(r => r.Status == RequestStatus.Assigned);
            }

            if (message.Criteria.IncludeCanceled)
            {
                results = results.Where(r => r.Status == RequestStatus.Canceled);
            }

            if (message.Criteria.EventId.HasValue)
            {
                results = results.Where(r => r.EventId == message.Criteria.EventId.Value);
            }

            if (!string.IsNullOrEmpty(message.Criteria.Keywords))
            {
                results = results.Where(r => 
                            r.Zip.Contains(message.Criteria.Keywords) || 
                            r.Address.Contains(message.Criteria.Keywords) ||
                            r.City.Contains(message.Criteria.Keywords) || 
                            r.Name.Contains(message.Criteria.Keywords));
            }

            // todo: sgordon: date added filtering

            return await results.Select(r => new RequestListModel
            {
                Id = r.RequestId,
                Name = r.Name,
                Address = r.Address,
                City = r.City,
                Postcode = r.Zip,
                Status = r.Status,
                DateAdded = r.DateAdded
            }).ToListAsync().ConfigureAwait(false);
        }
    }
}
