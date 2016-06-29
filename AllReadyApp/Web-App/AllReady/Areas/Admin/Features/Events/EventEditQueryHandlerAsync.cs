using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventEditQueryHandlerAsync : IAsyncRequestHandler<EventEditQuery, EventEditModel>
    {
        private readonly AllReadyContext _context;

        public EventEditQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventEditModel> Handle(EventEditQuery message)
        {
            EventEditModel result = null;

            var campaignEvent = await GetEvent(message);

            if (campaignEvent != null)
            {
                result = new EventEditModel
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
                    Location = campaignEvent.Location.ToEditModel(),
                    RequiredSkills = campaignEvent.RequiredSkills,
                    ImageUrl = campaignEvent.ImageUrl,
                    Headline = campaignEvent.Headline
                };
            }

            return result;
        }

        private async Task<Event> GetEvent(EventEditQuery message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(a => a.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Location)
                .SingleOrDefaultAsync(a => a.Id == message.EventId)
                .ConfigureAwait(false);
        }
    }
}