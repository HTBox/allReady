using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.Features.Tasks
{
    public class TaskSignupHandlerAsync : IAsyncRequestHandler<TaskSignupCommandAsync, TaskSignupResult>
    {
        private readonly IMediator _mediator;
        private readonly AllReadyContext _context;

        public TaskSignupHandlerAsync(IMediator mediator, AllReadyContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<TaskSignupResult> Handle(TaskSignupCommandAsync message)
        {
            var model = message.TaskSignupModel;

            var user = await _context.Users
                .Include(u => u.AssociatedSkills)
                .SingleOrDefaultAsync(u => u.Id == model.UserId).ConfigureAwait(false);

            var campaignEvent = await _context.Events
                .Include(a => a.RequiredSkills)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(s => s.Skill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers)
                .SingleOrDefaultAsync(a => a.Id == model.EventId).ConfigureAwait(false);

            if (campaignEvent == null)
            {
                return new TaskSignupResult { Status = TaskSignupResult.FAILURE_EVENTNOTFOUND };
            }

            var task = campaignEvent.Tasks.SingleOrDefault(t => t.Id == model.TaskId);

            if (task == null)
            {
                return new TaskSignupResult { Status = TaskSignupResult.FAILURE_TASKNOTFOUND };
            }

            if (task.IsClosed)
            {
                return new TaskSignupResult { Status = TaskSignupResult.FAILURE_CLOSEDTASK };
            }

            campaignEvent.UsersSignedUp = campaignEvent.UsersSignedUp ?? new List<EventSignup>();

            // If the user has not already been signed up for the event, sign them up
            if (!campaignEvent.UsersSignedUp.Any(eventSignup => eventSignup.User.Id == user.Id))
            {
                campaignEvent.UsersSignedUp.Add(new EventSignup
                {
                    Event = campaignEvent,
                    User = user,
                    PreferredEmail = model.PreferredEmail,
                    PreferredPhoneNumber = model.PreferredPhoneNumber,
                    AdditionalInfo = model.AdditionalInfo,
                    SignupDateTime = DateTime.UtcNow
                });
            }

            // If somehow the user has already been signed up for the task, don't sign them up again
            if (!task.AssignedVolunteers.Any(taskSignup => taskSignup.User.Id == user.Id))
            {
                task.AssignedVolunteers.Add(new TaskSignup
                {
                    Task = task,
                    User = user,
                    Status = TaskStatus.Accepted.ToString(),
                    StatusDateTimeUtc = DateTime.UtcNow,
                    PreferredEmail = model.PreferredEmail,
                    PreferredPhoneNumber = model.PreferredPhoneNumber,
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

            await _context.SaveChangesAsync().ConfigureAwait(false);

            //Notify admins of a new volunteer
            await _mediator.PublishAsync(new VolunteerSignupNotification { EventId = model.EventId, UserId = model.UserId, TaskId = task.Id })
                .ConfigureAwait(false);

            return new TaskSignupResult {Status = "success", Task = task};
        }
    }
}
