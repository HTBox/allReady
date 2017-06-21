using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class EventManagerInviteQueryHandler : IAsyncRequestHandler<EventManagerInviteQuery, EventManagerInviteViewModel>
    {
        private AllReadyContext _context;

        public EventManagerInviteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventManagerInviteViewModel> Handle(EventManagerInviteQuery message)
        {
            var @event = await _context.Events.AsNoTracking()
                .Include(e => e.Campaign).SingleAsync(e => e.Id == message.EventId);

            return new EventManagerInviteViewModel
            {
                EventId = message.EventId,
                EventName = @event.Name,
                CampaignId = @event.CampaignId,
                CampaignName = @event.Campaign.Name,
                OrganizationId = @event.Campaign.ManagingOrganizationId,
            };
        }
    }
}
