using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventRequestsQueryHandler : IAsyncRequestHandler<EventRequestsQuery, EventRequestsViewModel>
    {
        private readonly AllReadyContext _context;

        public EventRequestsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventRequestsViewModel> Handle(EventRequestsQuery message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(rec => rec.Campaign)
                .Select(rec => new EventRequestsViewModel
                {
                    EventId = rec.Id,
                    EventName = rec.Name,
                    CampaignId = rec.CampaignId,
                    CampaignName = rec.Campaign.Name
                })
                .Where(rec => rec.EventId == message.EventId)
                .SingleOrDefaultAsync();
        }
    }
}