using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Tasks
{
    public class VolunteerTaskSignupCommandHandler : IAsyncRequestHandler<VolunteerTaskSignupCommand, VolunteerTaskSignupResult>
    {
        private readonly IMediator _mediator;
        private readonly AllReadyContext _context;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public VolunteerTaskSignupCommandHandler(IMediator mediator, AllReadyContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<VolunteerTaskSignupResult> Handle(VolunteerTaskSignupCommand message)
        {
            var model = message.TaskSignupModel;

            var user = await _context.Users
                .Include(u => u.AssociatedSkills)
                .SingleOrDefaultAsync(u => u.Id == model.UserId);

            var @event = await _context.Events
                .Include(a => a.RequiredSkills)
                .Include(a => a.VolunteerTasks).ThenInclude(t => t.RequiredSkills).ThenInclude(s => s.Skill)
                .Include(a => a.VolunteerTasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(t => t.User)
                .SingleOrDefaultAsync(a => a.Id == model.EventId);

            if (@event == null)
            {
                return new VolunteerTaskSignupResult { Status = TaskResultStatus.Failure_EventNotFound };
            }

            var volunteerTask = @event.VolunteerTasks.SingleOrDefault(t => t.Id == model.VolunteerTaskId);
            if (volunteerTask == null)
            {
                return new VolunteerTaskSignupResult { Status = TaskResultStatus.Failure_TaskNotFound };
            }

            if (volunteerTask.IsClosed)
            {
                return new VolunteerTaskSignupResult { Status = TaskResultStatus.Failure_ClosedTask };
            }

            // If somehow the user has already been signed up for the task, don't sign them up again
            if (volunteerTask.AssignedVolunteers.All(volunteerTaskSignup => volunteerTaskSignup.User.Id != user.Id))
            {
                volunteerTask.AssignedVolunteers.Add(new VolunteerTaskSignup
                {
                    VolunteerTask = volunteerTask,
                    User = user,
                    Status = Models.VolunteerTaskStatus.Accepted,
                    StatusDateTimeUtc = DateTimeUtcNow(),
                    AdditionalInfo = model.AdditionalInfo
                });
            }

            //Add selected new skills (if any) to the current user
            if (model.AddSkillIds.Count > 0)
            {
                var skillsToAdd = volunteerTask.RequiredSkills
                    .Where(taskSkill => model.AddSkillIds.Contains(taskSkill.SkillId))
                    .Select(taskSkill => new UserSkill { SkillId = taskSkill.SkillId, UserId = user.Id });

                user.AssociatedSkills.AddRange(skillsToAdd.Where(skillToAdd => user.AssociatedSkills.All(userSkill => userSkill.SkillId != skillToAdd.SkillId)));

                _context.Update(user);
            }

            await _context.SaveChangesAsync();

            //Notify admins of a new volunteer
            await _mediator.PublishAsync(new VolunteerSignedUpNotification { UserId = model.UserId, VolunteerTaskId = volunteerTask.Id });

            return new VolunteerTaskSignupResult {Status = TaskResultStatus.Success, VolunteerTask = volunteerTask};
        }
    }
}
