using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventSummaryQueryHandlerAsync : IAsyncRequestHandler<EventSummaryQuery, EventSummaryModel>
    {
        private readonly AllReadyContext _context;

        public EventSummaryQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventSummaryModel> Handle(EventSummaryQuery message)
        {
            EventSummaryModel result = null;

            var campaignEvent = await GetEvent(message);

            if (campaignEvent != null)
            {
                result = new EventSummaryModel
                {
                    Id = campaignEvent.Id,
                    EventType = campaignEvent.EventType,
                    CampaignName = campaignEvent.Campaign.Name,
                    CampaignId = campaignEvent.Campaign.Id,
                    OrganizationId = campaignEvent.Campaign.ManagingOrganizationId,
                    OrganizationName = campaignEvent.Campaign.ManagingOrganization.Name,
                    Name = campaignEvent.Name,
                    Description = campaignEvent.Description,
                    TimeZoneId = campaignEvent.Campaign.TimeZoneId,
                    StartDateTime = campaignEvent.StartDateTime,
                    EndDateTime = campaignEvent.EndDateTime,
                    NumberOfVolunteersRequired = campaignEvent.NumberOfVolunteersRequired,
                    IsLimitVolunteers = campaignEvent.IsLimitVolunteers,
                    IsAllowWaitList = campaignEvent.IsAllowWaitList,
                    ImageUrl = campaignEvent.ImageUrl,
                };
            }

            return result;
        }

        private async Task<Event> GetEvent(EventSummaryQuery message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .SingleOrDefaultAsync(a => a.Id == message.EventId)
                .ConfigureAwait(false);
        }
    }
}