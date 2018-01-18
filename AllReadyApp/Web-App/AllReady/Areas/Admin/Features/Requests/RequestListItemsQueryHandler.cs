using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Itinerary;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestListItemsQueryHandler : IAsyncRequestHandler<RequestListItemsQuery, List<RequestListViewModel>>
    {
        private readonly AllReadyContext _context;

        public RequestListItemsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<RequestListViewModel>> Handle(RequestListItemsQuery message)
        {
            var results = _context.Requests.AsNoTracking();

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

            if (message.Criteria.ItineraryId.HasValue)
            {
                results = results.Include(x => x.Itinerary).Where(r => r.Itinerary.ItineraryId == message.Criteria.ItineraryId.Value);
            }

            if (message.Criteria.Status.HasValue)
            {
                results = results.Where(r => r.Status == message.Criteria.Status);
            }

            if (!string.IsNullOrEmpty(message.Criteria.Keywords))
            {
                results = results.Where(r => 
                            r.PostalCode.Contains(message.Criteria.Keywords) || 
                            r.Address.Contains(message.Criteria.Keywords) ||
                            r.City.Contains(message.Criteria.Keywords) || 
                            r.Name.Contains(message.Criteria.Keywords));
            }

            if (message.Criteria.OrganizationId.HasValue)
            {
                results = results.Where(r => r.OrganizationId == message.Criteria.OrganizationId);
            }

            // todo: sgordon: date added filtering

            return await results.Select(r => new RequestListViewModel
            {
                Id = r.RequestId,
                Name = r.Name,
                Address = r.Address,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                City = r.City,
                PostalCode = r.PostalCode,
                Status = r.Status,
                DateAdded = r.DateAdded,
            }).ToListAsync();
        }
    }
}
