using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.TeamLead;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.TeamLead
{
    public class TeamLeadItineraryListViewModelQuery : IAsyncRequest<TeamLeadItineraryListerViewModel>
    {
        public TeamLeadItineraryListViewModelQuery(ClaimsPrincipal user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }

        public ClaimsPrincipal User { get; }
    }

    public class TeamLeadItineraryListViewModelQueryHandler : IAsyncRequestHandler<TeamLeadItineraryListViewModelQuery, TeamLeadItineraryListerViewModel>
    {
        private readonly AllReadyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeamLeadItineraryListViewModelQueryHandler(AllReadyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<TeamLeadItineraryListerViewModel> Handle(TeamLeadItineraryListViewModelQuery message)
        {
            var user = await _userManager.GetUserAsync(message.User);

            if (user == null)
            {
                return new TeamLeadItineraryListerViewModel();
            }
            
            var userItineraries = await _context.VolunteerTaskSignups
                .AsNoTracking()
                .Include(vts => vts.Itinerary).ThenInclude(i => i.Event).ThenInclude(e => e.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Where(vts => vts.IsTeamLead && vts.User.Id == user.Id && vts.Itinerary != null)
                .Select(vts => new
                {
                    CampaignName = vts.Itinerary.Event.Campaign.Name,
                    EventName = vts.Itinerary.Event.Name,
                    ItineraryName = vts.Itinerary.Name,
                    ItineraryId = vts.Itinerary.Id,
                    ItineraryDate = vts.Itinerary.Date,
                    TimeZoneId = vts.Itinerary.Event.TimeZoneId
                })
                .ToListAsync();

            var groupedItineraries = userItineraries
                .GroupBy(x => new {x.EventName, x.CampaignName})
                .GroupBy(x => x.Key.CampaignName);

            var viewModel = new TeamLeadItineraryListerViewModel();

            foreach (var campaignGroup in groupedItineraries)
            {
                var campaignItem = new TeamLeadItineraryListerCampaign
                {
                    Name = campaignGroup.Key
                };

                foreach (var eventGroup in campaignGroup)
                {
                    {
                        var eventItem = new TeamLeadItineraryListerEvent
                        {
                            Name = eventGroup.Key.EventName
                        };

                        foreach (var itinerary in eventGroup)
                        {
                            var itineraryItem = new TeamLeadItineraryListerItinerary
                            {
                                Name = itinerary.ItineraryName,
                                Date = itinerary.ItineraryDate,
                                Id = itinerary.ItineraryId,
                                TimeZoneId = itinerary.TimeZoneId
                            };

                            eventItem.Itineraries.Add(itineraryItem);
                        }

                        campaignItem.CampaignEvents.Add(eventItem);
                    }
                }

                viewModel.Campaigns.Add(campaignItem);
            }

            return viewModel;
        }
    }
}
