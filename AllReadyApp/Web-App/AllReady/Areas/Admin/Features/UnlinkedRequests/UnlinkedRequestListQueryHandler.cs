using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.UnlinkedRequests;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.UnlinkedRequests
{
    public class UnlinkedRequestListQueryHandler :
        IAsyncRequestHandler<UnlinkedRequestListQuery, UnlinkedRequestViewModel>
    {
        private readonly AllReadyContext _context;

        public UnlinkedRequestListQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<UnlinkedRequestViewModel> Handle(UnlinkedRequestListQuery message)
        {
            var requests = await _context.Requests
                .AsNoTracking()
                .Where(r => r.OrganizationId == message.OrganizationId && !r.EventId.HasValue)
                .Select(r => new RequestSelectViewModel()
                {
                    Id = r.RequestId,
                    Name = r.Name,
                    Address = r.Address,
                    City = r.City,
                    DateAdded = r.DateAdded,
                    PostalCode = r.PostalCode
                })
                .ToListAsync();

            var events = await _context.Events
                .AsNoTracking()    
                .Where(e => e.Campaign.ManagingOrganizationId == message.OrganizationId)
                .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Campaign.ManagingOrganization.Name} > {e.Campaign.Name} > {e.Name}"
                    }
                ).ToListAsync();

            return new UnlinkedRequestViewModel()
            {
                Requests = requests,
                Events = events
            };
        }
    }
}