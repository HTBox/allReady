using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DupicateEventQueryHandler : IAsyncRequestHandler<DuplicateEventQuery, DuplicateEventModel>
    {
        private readonly AllReadyContext _context;

        public DupicateEventQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DuplicateEventModel> Handle(DuplicateEventQuery message)
        {
            DuplicateEventModel result = null;

            var campaignEvent = await GetEvent(message);

            if (campaignEvent != null)
            {
                result = new DuplicateEventModel
                {
                    Id = campaignEvent.Id,
                    CampaignName = campaignEvent.Campaign.Name,
                    CampaignId = campaignEvent.Campaign.Id,
                    OrganizationId = campaignEvent.Campaign.ManagingOrganizationId,
                    Name = campaignEvent.Name,
                    Description = campaignEvent.Description,
                    TimeZoneId = campaignEvent.Campaign.TimeZoneId,
                    StartDateTime = campaignEvent.StartDateTime,
                    EndDateTime = campaignEvent.EndDateTime,
                };
            }

            return result;
        }

        private async Task<Event> GetEvent(DuplicateEventQuery message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .SingleOrDefaultAsync(a => a.Id == message.EventId)
                .ConfigureAwait(false);
        }
    }
}