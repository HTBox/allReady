using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.Features.Tasks
{
    public class TaskSignupCommandHandler : IAsyncRequestHandler<TaskSignupCommand, TaskSignupResult>
    {
        private readonly IMediator _mediator;
        private readonly AllReadyContext _context;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public TaskSignupCommandHandler(IMediator mediator, AllReadyContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<TaskSignupResult> Handle(TaskSignupCommand message)
        {
            var model = message.TaskSignupModel;

            var user = await _context.Users
                .Include(u => u.AssociatedSkills)
                .SingleOrDefaultAsync(u => u.Id == model.UserId);

            var @event = await _context.Events
                .Include(a => a.RequiredSkills)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(s => s.Skill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(t => t.User)
                .SingleOrDefaultAsync(a => a.Id == model.EventId);

            if (@event == null)
            {
                return new TaskSignupResult { Status = TaskSignupResult.FAILURE_EVENTNOTFOUND };
            }

            var task = @event.Tasks.SingleOrDefault(t => t.Id == model.TaskId);
            if (task == null)
            {
                return new TaskSignupResult { Status = TaskSignupResult.FAILURE_TASKNOTFOUND };
            }

            if (task.IsClosed)
            {
                return new TaskSignupResult { Status = TaskSignupResult.FAILURE_CLOSEDTASK };
            }

            // If somehow the user has already been signed up for the task, don't sign them up again
            if (task.AssignedVolunteers.All(taskSignup => taskSignup.User.Id != user.Id))
            {
                task.AssignedVolunteers.Add(new TaskSignup
                {
                    Task = task,
                    User = user,
                    Status = TaskStatus.Accepted.ToString(),
                    StatusDateTimeUtc = DateTimeUtcNow(),
                    AdditionalInfo = model.AdditionalInfo
                });
            }

            //Add selected new skills (if any) to the current user
            if (model.AddSkillIds.Count > 0)
            {
                var skillsToAdd = task.RequiredSkills
                    .Where(taskSkill => model.AddSkillIds.Contains(taskSkill.SkillId))
                    .Select(taskSkill => new UserSkill { SkillId = taskSkill.SkillId, UserId = user.Id });

                user.AssociatedSkills.AddRange(skillsToAdd.Where(skillToAdd => user.AssociatedSkills.All(userSkill => userSkill.SkillId != skillToAdd.SkillId)));

                _context.Update(user);
            }

            await _context.SaveChangesAsync();

            //Notify admins of a new volunteer
            await _mediator.PublishAsync(new VolunteerSignedUpNotification { UserId = model.UserId, TaskId = task.Id });

            return new TaskSignupResult {Status = "success", Task = task};
        }
    }
}