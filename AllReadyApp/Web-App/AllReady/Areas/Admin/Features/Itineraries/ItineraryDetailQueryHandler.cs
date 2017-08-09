using AllReady.Models;
using AllReady.Services;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQueryHandler : IAsyncRequestHandler<ItineraryDetailQuery, ItineraryDetailsViewModel>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        private const string BingMapsUrl = "https://www.bing.com/maps";

        public ItineraryDetailQueryHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<ItineraryDetailsViewModel> Handle(ItineraryDetailQuery message)
        {
            var itineraryDetails = await _context.Itineraries
                .AsNoTracking()
                .Include(x => x.StartLocation)
                .Include(x => x.EndLocation)
                .Include(x => x.Event).ThenInclude(x => x.Campaign).ThenInclude(x => x.ManagingOrganization)
                .Include(x => x.TeamMembers).ThenInclude(x => x.VolunteerTask)
                .Include(x => x.Requests).ThenInclude(x => x.Request)
                .Where(a => a.Id == message.ItineraryId)
                .Select(i => new ItineraryDetailsViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Date = i.Date,
                    EventId = i.EventId,
                    EventName = i.Event.Name,
                    CampaignId = i.Event.Campaign.Id,
                    CampaignName = i.Event.Campaign.Name,
                    OrganizationId = i.Event.Campaign.ManagingOrganizationId,
                    StartAddress = i.StartLocation != null ? i.StartLocation.FullAddress : null,
                    EndAddress = GetEndAddress(i.StartLocation, i.EndLocation, i.UseStartAddressAsEndAddress),
                    UseStartAddressAsEndAddress = i.UseStartAddressAsEndAddress,
                    TeamMembers = i.TeamMembers
                        .OrderBy(tm => tm.IsTeamLead != true)
                        .ThenBy(tm => tm.User.LastName)
                        .ThenBy(tm => tm.User.FirstName)
                        .Select(tm => new TeamListViewModel
                        {
                            VolunteerTaskSignupId = tm.Id,
                            VolunteerEmail = tm.User.Email,
                            VolunteerTaskName = tm.VolunteerTask.Name,
                            FullName = !string.IsNullOrWhiteSpace(tm.User.Name) ? $"{tm.User.LastName}, {tm.User.FirstName}" : "* Name Missing *",
                            IsTeamLead = tm.IsTeamLead
                        }).ToList(),
                    Requests = i.Requests.OrderBy(r => r.OrderIndex).Select(r => new RequestListViewModel
                    {
                        Id = r.Request.RequestId,
                        Name = r.Request.Name,
                        Address = r.Request.Address,
                        City = r.Request.City,
                        Status = r.Request.Status,
                        Longitude = r.Request.Longitude,
                        Latitude = r.Request.Latitude
                    }).ToList()
                })
                .SingleOrDefaultAsync();

            if (itineraryDetails == null) return null;

            itineraryDetails.PotentialTeamMembers = await _mediator.SendAsync(new PotentialItineraryTeamMembersQuery { EventId = itineraryDetails.EventId, Date = itineraryDetails.Date });

            var potentialTeamMembers = itineraryDetails.PotentialTeamMembers.ToList();

            itineraryDetails.HasPotentialTeamMembers = potentialTeamMembers.Any();
            itineraryDetails.PotentialTeamMembers = potentialTeamMembers.AddNullOptionToFront("<Please select your next team member>");

            var potentialTeamLeads = itineraryDetails.TeamMembers.Where(t => !t.IsTeamLead).Select(x => new SelectListItem { Text = x.FullName, Value = x.VolunteerTaskSignupId.ToString() }).ToList();

            if (potentialTeamLeads.Any() && itineraryDetails.HasTeamLead)
            {
                itineraryDetails.PotentialTeamLeads = potentialTeamLeads.AddNullOptionToFront("<Select new team lead>");
            }
            else if (potentialTeamLeads.Any())
            {
                itineraryDetails.PotentialTeamLeads = potentialTeamLeads.AddNullOptionToFront("<Select a team lead>");
            }

            if (!string.IsNullOrWhiteSpace(itineraryDetails.StartAddress) && !string.IsNullOrWhiteSpace(itineraryDetails.EndAddress))
            {
                itineraryDetails.CanOptimizeAndDisplayRoute = true;
            }

            if (itineraryDetails.Requests.Any())
            {
                itineraryDetails.Requests[0].IsFirst = true;
                itineraryDetails.Requests[itineraryDetails.Requests.Count - 1].IsLast = true;
                itineraryDetails.HasAnyRequests = true;

                BuildBingUrl(itineraryDetails);
            }

            return itineraryDetails;
        }

        private string GetEndAddress(Location startLocation, Location endLocation, bool useStartAsEnd)
        {
            if (useStartAsEnd)
            {
                if (startLocation != null)
                {
                    return startLocation.FullAddress;
                }
            }
            else
            {
                if (endLocation != null)
                {
                    return endLocation.FullAddress;
                }
            }

            return null;
        }

        private static void BuildBingUrl(ItineraryDetailsViewModel model)
        {
            var bingMapUrl = new StringBuilder(BingMapsUrl).Append("?rtp=adr.");
            bingMapUrl.Append(model.StartAddress);

            foreach (var req in model.Requests)
            {
                bingMapUrl.Append("~pos.").Append(req.Latitude).Append("_").Append(req.Longitude).Append("_").Append(req.Name);
            }

            bingMapUrl.Append("~adr.").Append(model.EndAddress);
            bingMapUrl.Append("&rtop=0~1~0");

            model.BingMapUrl = bingMapUrl.ToString();
        }
    }
}
