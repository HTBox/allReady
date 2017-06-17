using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventListerQueryHandler : IAsyncRequestHandler<EventListerQuery, List<EventListerViewModel>>
    {
        private readonly AllReadyContext _context;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public EventListerQueryHandler(AllReadyContext context, IUserAuthorizationService userAuthorizationService)
        {
            _context = context;
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<List<EventListerViewModel>> Handle(EventListerQuery message)
        {
            var eventIds = await _context.EventManagers
                .AsNoTracking()
                .Where(x => x.UserId == message.UserId)
                .Select(x => x.EventId)
                .ToListAsync();

            eventIds.AddRange(await _context.CampaignManagers
                .AsNoTracking()
                .Where(x => x.UserId == message.UserId)
                .SelectMany(x => x.Campaign.Events.Select(e => e.Id))
                .ToListAsync());

            var administeredOrgId = _userAuthorizationService.GetOrganizationId;

            if (administeredOrgId != null)
            {
                eventIds.AddRange(await _context.Events
                    .AsNoTracking()
                    .Include(x => x.Campaign)
                    .Where(x => x.Campaign.ManagingOrganizationId == administeredOrgId)
                    .Select(x => x.Id)
                    .ToListAsync());
            }

            if (_userAuthorizationService.IsSiteAdmin)
            {
                eventIds.AddRange(await _context.Events
                    .AsNoTracking()                 
                    .Select(x => x.Id)
                    .ToListAsync());
            }

            var organizationEvents = await _context.Events
                .AsNoTracking()
                .Include(x => x.Campaign).ThenInclude(x => x.ManagingOrganization)
                .Where(x => eventIds.Distinct().Contains(x.Id))
                .OrderBy(x => x.Campaign.Id)
                .Select(x => new
                    {
                        EventId = x.Id,
                        Name = x.Name,
                        Campaign = x.Campaign.Name,
                        Organization = x.Campaign.ManagingOrganization.Name
                    })
                .GroupBy(x => x.Organization)
                .ToListAsync();

            var eventListViewModels = new List<EventListerViewModel>();

            foreach(var orgEvent in organizationEvents)
            {
                eventListViewModels.Add(new EventListerViewModel
                {
                    Organization = orgEvent.Key,
                    Events = orgEvent.Select(x => new EventListItemViewModel { CampaignName = x.Campaign, EventId = x.EventId, Name = x.Name }).ToList()
                });
            }

            return eventListViewModels;
        }
    }
}

