using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AllReady.Extensions;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditVolunteerTaskCommandHandler : IAsyncRequestHandler<EditVolunteerTaskCommand, int>
    {
        private readonly AllReadyContext _context;

        public EditVolunteerTaskCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditVolunteerTaskCommand message)
        {
            var volunteerTask = await _context.VolunteerTasks.Include(t => t.RequiredSkills).SingleOrDefaultAsync(t => t.Id == message.VolunteerTask.Id) ?? new VolunteerTask();

            volunteerTask.Name = message.VolunteerTask.Name;
            volunteerTask.Description = message.VolunteerTask.Description;
            volunteerTask.Event = _context.Events.SingleOrDefault(a => a.Id == message.VolunteerTask.EventId);
            volunteerTask.Organization = _context.Organizations.SingleOrDefault(t => t.Id == message.VolunteerTask.OrganizationId);

            volunteerTask.StartDateTime = message.VolunteerTask.StartDateTime;
            volunteerTask.EndDateTime = message.VolunteerTask.EndDateTime;

            volunteerTask.NumberOfVolunteersRequired = message.VolunteerTask.NumberOfVolunteersRequired;
            volunteerTask.IsLimitVolunteers = volunteerTask.Event.IsLimitVolunteers;
            volunteerTask.IsAllowWaitList = volunteerTask.Event.IsAllowWaitList;

            if (volunteerTask.Id > 0)
            {
                var volunteerTaskSkillsToRemove = _context.VolunteerTaskSkills.Where(ts => ts.VolunteerTaskId == volunteerTask.Id && (message.VolunteerTask.RequiredSkills == null || message.VolunteerTask.RequiredSkills.All(ts1 => ts1.SkillId != ts.SkillId)));
                _context.VolunteerTaskSkills.RemoveRange(volunteerTaskSkillsToRemove);
            }

            if (message.VolunteerTask.RequiredSkills != null)
            {
                volunteerTask.RequiredSkills.AddRange(message.VolunteerTask.RequiredSkills.Where(mt => volunteerTask.RequiredSkills.All(ts => ts.SkillId != mt.SkillId)));
            }

            _context.AddOrUpdate(volunteerTask);

            await _context.SaveChangesAsync();

            return volunteerTask.Id;
        }
    }
}