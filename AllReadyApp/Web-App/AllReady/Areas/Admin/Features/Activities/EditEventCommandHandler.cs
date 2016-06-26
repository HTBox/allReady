using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EditEventCommandHandler : IAsyncRequestHandler<EditEventCommand, int>
    {
        private AllReadyContext _context;

        public EditEventCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        public async Task<int> Handle(EditEventCommand message)
        {
            var campaignEvent = await GetEvent(message);

            if (campaignEvent == null)
            {
                campaignEvent = new Event();
            }

            campaignEvent.Name = message.Event.Name;
            campaignEvent.Description = message.Event.Description;
            campaignEvent.EventType = message.Event.EventType;

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(message.Event.TimeZoneId);
            var startDateTimeOffset = timeZone.GetUtcOffset(message.Event.StartDateTime);
            campaignEvent.StartDateTime = new DateTimeOffset(message.Event.StartDateTime.Year, message.Event.StartDateTime.Month, message.Event.StartDateTime.Day, message.Event.StartDateTime.Hour, message.Event.StartDateTime.Minute, 0, startDateTimeOffset);

            var endDateTimeOffset = timeZone.GetUtcOffset(message.Event.EndDateTime);
            campaignEvent.EndDateTime = new DateTimeOffset(message.Event.EndDateTime.Year, message.Event.EndDateTime.Month, message.Event.EndDateTime.Day, message.Event.EndDateTime.Hour, message.Event.EndDateTime.Minute, 0, endDateTimeOffset);
            campaignEvent.CampaignId = message.Event.CampaignId;
            
            campaignEvent.ImageUrl = message.Event.ImageUrl;
            campaignEvent.NumberOfVolunteersRequired = message.Event.NumberOfVolunteersRequired;

            if (campaignEvent.IsLimitVolunteers != message.Event.IsLimitVolunteers || campaignEvent.IsAllowWaitList != message.Event.IsAllowWaitList)
            {
                campaignEvent.IsAllowWaitList = message.Event.IsAllowWaitList;
                campaignEvent.IsLimitVolunteers = message.Event.IsLimitVolunteers;
                
                // cascade values to all tasks associated with this event
                foreach (var task in _context.Tasks.Where(task => task.Event.Id == campaignEvent.Id))
                {
                    task.IsLimitVolunteers = campaignEvent.IsLimitVolunteers;
                    task.IsAllowWaitList = campaignEvent.IsAllowWaitList;
                    _context.Update(task);
                }
            }

            if (campaignEvent.Id > 0)
            {
                var skillsToRemove = _context.EventSkills.Where(skill => skill.EventId == campaignEvent.Id && (message.Event.RequiredSkills == null ||
                    !message.Event.RequiredSkills.Any(ts1 => ts1.SkillId == skill.SkillId)));
                _context.EventSkills.RemoveRange(skillsToRemove);
            }

            if (message.Event.RequiredSkills != null)
            {
                campaignEvent.RequiredSkills.AddRange(message.Event.RequiredSkills.Where(mt => !campaignEvent.RequiredSkills.Any(ts => ts.SkillId == mt.SkillId)));
            }

            if (message.Event.Location != null)
            {
                campaignEvent.Location = campaignEvent.Location.UpdateModel(message.Event.Location);
                _context.Update(campaignEvent.Location);
            }

            _context.Update(campaignEvent);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return campaignEvent.Id;
        }

        private async Task<Event> GetEvent(EditEventCommand message)
        {
            return await _context.Events
                .Include(a => a.RequiredSkills)
                .SingleOrDefaultAsync(c => c.Id == message.Event.Id)
                .ConfigureAwait(false);
        }
    }
}