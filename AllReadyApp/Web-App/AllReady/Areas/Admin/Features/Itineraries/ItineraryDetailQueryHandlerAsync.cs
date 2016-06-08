using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQueryHandlerAsync : IAsyncRequestHandler<ItineraryDetailQuery, ItineraryDetailsModel>
    {
        private readonly AllReadyContext _context;

        public ItineraryDetailQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<ItineraryDetailsModel> Handle(ItineraryDetailQuery message)
        {
            var itineraryDetails = await _context.Itineraries
                .AsNoTracking()
                .Include(x => x.Event)
                .Include(x => x.Event.Campaign.ManagingOrganization)
                .Include(x => x.TeamMembers).ThenInclude(x => x.Task)
                .Where(a => a.Id == message.ItineraryId)
                .Select(i => new ItineraryDetailsModel {
                    Id = i.Id,
                    Name = i.Name,
                    Date = i.Date,
                    EventId = i.EventId,
                    EventName = i.Event.Name,
                    OrganizationId = i.Event.Campaign.ManagingOrganizationId,
                    TeamMembers = i.TeamMembers.Select(tm => new TeamListModel
                    {
                        VolunteerEmail = tm.User.Email,
                        TaskName = tm.Task.Name
                    }).ToList()
                })
                .SingleOrDefaultAsync().ConfigureAwait(false);                

            return itineraryDetails;
        }

        // todo: sgordon: this could be more efficient, if we select only the fields we need from the query
        private async Task<Itinerary> GetItinerary(ItineraryDetailQuery message)
        {
            return await _context.Itineraries
                .AsNoTracking()    
                .Include(x => x.Event)           
                .Include(x => x.Event.Campaign.ManagingOrganization)
                .Include(x => x.TeamMembers).ThenInclude(x => x.Task)
                .Include(x => x.TeamMembers).ThenInclude(x => x.User)
                .SingleOrDefaultAsync(a => a.Id == message.ItineraryId)
                .ConfigureAwait(false);
        }
    }
}
