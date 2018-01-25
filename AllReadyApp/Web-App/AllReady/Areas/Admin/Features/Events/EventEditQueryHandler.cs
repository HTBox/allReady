using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventEditQueryHandler : IAsyncRequestHandler<EventEditQuery, EventEditViewModel>
    {
        private readonly AllReadyContext _context;

        public EventEditQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventEditViewModel> Handle(EventEditQuery message)
        {
            EventEditViewModel result = null;

            var campaignEvent = await GetEvent(message);

            if (campaignEvent != null)
            {
                result = new EventEditViewModel
                {
                    Id = campaignEvent.Id,
                    EventType = campaignEvent.EventType,
                    CampaignName = campaignEvent.Campaign.Name,
                    CampaignId = campaignEvent.Campaign.Id,
                    CampaignStartDateTime = campaignEvent.Campaign.StartDateTime,
                    CampaignEndDateTime = campaignEvent.Campaign.EndDateTime,
                    OrganizationId = campaignEvent.Campaign.ManagingOrganizationId,
                    OrganizationName = campaignEvent.Campaign.ManagingOrganization.Name,
                    Name = campaignEvent.Name,
                    Description = campaignEvent.Description,
                    TimeZoneId = campaignEvent.TimeZoneId,
                    StartDateTime = campaignEvent.StartDateTime,
                    EndDateTime = campaignEvent.EndDateTime,
                    IsLimitVolunteers = campaignEvent.IsLimitVolunteers,
                    IsAllowWaitList = campaignEvent.IsAllowWaitList,
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
                .SingleOrDefaultAsync(a => a.Id == message.EventId);
        }
    }
}
