using System.Linq;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Providers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EditEventCommandHandler : IAsyncRequestHandler<EditEventCommand, int>
    {
        private AllReadyContext _context;
        private readonly IConvertDateTimeOffset _dateTimeOffsetConverter;

        public EditEventCommandHandler(AllReadyContext context, IConvertDateTimeOffset dateTimeOffsetConverter)
        {
            _context = context;
            _dateTimeOffsetConverter = dateTimeOffsetConverter;
        }
        public async Task<int> Handle(EditEventCommand message)
        {
            var campaignEvent = await GetEvent(message) ?? new Event();

            campaignEvent.Name = message.Event.Name;
            campaignEvent.Description = message.Event.Description;
            campaignEvent.EventType = message.Event.EventType;

            campaignEvent.StartDateTime = _dateTimeOffsetConverter.ConvertDateTimeOffsetTo(message.Event.TimeZoneId, message.Event.StartDateTime, message.Event.StartDateTime.Hour, message.Event.StartDateTime.Minute);
            campaignEvent.EndDateTime = _dateTimeOffsetConverter.ConvertDateTimeOffsetTo(message.Event.TimeZoneId, message.Event.EndDateTime, message.Event.EndDateTime.Hour, message.Event.EndDateTime.Minute);

            campaignEvent.CampaignId = message.Event.CampaignId;
            
            campaignEvent.ImageUrl = message.Event.ImageUrl;

            if (campaignEvent.IsLimitVolunteers != message.Event.IsLimitVolunteers || campaignEvent.IsAllowWaitList != message.Event.IsAllowWaitList)
            {
                campaignEvent.IsAllowWaitList = message.Event.IsAllowWaitList;
                campaignEvent.IsLimitVolunteers = message.Event.IsLimitVolunteers;
                
                // cascade values to all tasks associated with this event
                foreach (var task in campaignEvent.Tasks)
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
                _context.AddOrUpdate(campaignEvent.Location);
            }

            campaignEvent.Headline = message.Event.Headline;

            _context.AddOrUpdate(campaignEvent);
            await _context.SaveChangesAsync();

            return campaignEvent.Id;
        }

        private async Task<Event> GetEvent(EditEventCommand message)
        {
            return await _context.Events
                .Include(a => a.RequiredSkills)
                .Include(a => a.Tasks)
                .SingleOrDefaultAsync(c => c.Id == message.Event.Id);
        }
    }
}