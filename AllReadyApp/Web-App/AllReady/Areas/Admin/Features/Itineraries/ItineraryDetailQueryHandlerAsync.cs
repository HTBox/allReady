using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQueryHandlerAsync : IAsyncRequestHandler<ItineraryDetailQuery, ItineraryDetailsModel>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public ItineraryDetailQueryHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<ItineraryDetailsModel> Handle(ItineraryDetailQuery message)
        {
            var itineraryDetails = await _context.Itineraries
                .AsNoTracking()
                .Include(x => x.Event).ThenInclude(x => x.Campaign)
                .Include(x => x.Event.Campaign.ManagingOrganization)
                .Include(x => x.TeamMembers).ThenInclude(x => x.Task)
                .Include(x => x.Requests).ThenInclude(x => x.Request)
                .Where(a => a.Id == message.ItineraryId)
                .Select(i => new ItineraryDetailsModel {
                    Id = i.Id,
                    Name = i.Name,
                    Date = i.Date,
                    EventId = i.EventId,
                    EventName = i.Event.Name,
                    CampaignId = i.Event.Campaign.Id,
                    CampaignName = i.Event.Campaign.Name,
                    OrganizationId = i.Event.Campaign.ManagingOrganizationId,
                    TeamMembers = i.TeamMembers.Select(tm => new TeamListModel
                    {
                        VolunteerEmail = tm.User.Email,
                        TaskName = tm.Task.Name,
                        //FullName = string.Concat(tm.User.Name)
                        FullName = string.Format(tm.User.Forename, tm.User.Surname, "{0} {1}")
                    }).ToList(),
                    Requests = i.Requests.Select(r => new RequestListModel
                    {
                        Name = r.Request.Name,
                        Address = r.Request.Address,
                        City = r.Request.City,
                        Status = r.Request.Status.ToString()
                    }).ToList()
                })
                .SingleOrDefaultAsync().ConfigureAwait(false);

            if (itineraryDetails == null) return null;

            itineraryDetails.PotentialTeamMembers = await _mediator.SendAsync(new PotentialItineraryTeamMembersQuery { EventId = itineraryDetails.EventId, Date = itineraryDetails.Date });
            itineraryDetails.HasPotentialTeamMembers = itineraryDetails.PotentialTeamMembers.Count() > 0;
            itineraryDetails.PotentialTeamMembers = itineraryDetails.PotentialTeamMembers.AddNullOptionToFront("<Please select your next team member>");

            return itineraryDetails;
        }
    }
}
