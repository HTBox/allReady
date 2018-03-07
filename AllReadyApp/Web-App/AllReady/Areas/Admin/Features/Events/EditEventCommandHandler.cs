using System.Linq;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                _context.Events.Add(campaignEvent);
            }

            campaignEvent.Name = message.Event.Name;
            campaignEvent.Description = message.Event.Description;
            campaignEvent.EventType = message.Event.EventType;

            campaignEvent.TimeZoneId = message.Event.TimeZoneId;
            campaignEvent.StartDateTime = message.Event.StartDateTime;
            campaignEvent.EndDateTime = message.Event.EndDateTime;

            campaignEvent.CampaignId = message.Event.CampaignId;
            
            campaignEvent.ImageUrl = message.Event.ImageUrl;

            if (campaignEvent.IsLimitVolunteers != message.Event.IsLimitVolunteers || campaignEvent.IsAllowWaitList != message.Event.IsAllowWaitList)
            {
                campaignEvent.IsAllowWaitList = message.Event.IsAllowWaitList;
                campaignEvent.IsLimitVolunteers = message.Event.IsLimitVolunteers;
                
                // cascade values to all volunteerTasks associated with this event
                foreach (var volunteerTask in campaignEvent.VolunteerTasks)
                {
                    volunteerTask.IsLimitVolunteers = campaignEvent.IsLimitVolunteers;
                    volunteerTask.IsAllowWaitList = campaignEvent.IsAllowWaitList;
                    _context.Update(volunteerTask);
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
            }
            campaignEvent.Headline = message.Event.Headline;
            await _context.SaveChangesAsync();
            return campaignEvent.Id;
        }

        private async Task<Event> GetEvent(EditEventCommand message)
        {
            return await _context.Events
                .Include(a => a.RequiredSkills)
                .Include(a => a.VolunteerTasks)
                .Include(a => a.Location)
                .SingleOrDefaultAsync(c => c.Id == message.Event.Id);
        }
    }
}
