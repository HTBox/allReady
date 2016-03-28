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
    public class TaskSignupHandler : IAsyncRequestHandler<TaskSignupCommand, TaskSignupResult>
    {
        private readonly IMediator _bus;
        private readonly AllReadyContext _context;

        public TaskSignupHandler(IMediator bus, AllReadyContext context)
        {
            _bus = bus;
            _context = context;
        }

        public async Task<TaskSignupResult> Handle(TaskSignupCommand message)
        {
            var model = message.TaskSignupModel;

            var user = await _context.Users
                .Include(u => u.AssociatedSkills)
                .SingleOrDefaultAsync(u => u.Id == model.UserId);

            var activity = await _context.Activities
                .Include(a => a.RequiredSkills)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(s => s.Skill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers)
                .SingleOrDefaultAsync(a => a.Id == model.ActivityId);

            var task = activity.Tasks.SingleOrDefault(t => t.Id == model.TaskId);

            activity.UsersSignedUp = activity.UsersSignedUp ?? new List<ActivitySignup>();

            // If the user has not already been signed up for the activity, sign them up
            if (!activity.UsersSignedUp.Any(activitySignup => activitySignup.User.Id == user.Id))
            {
                activity.UsersSignedUp.Add(new ActivitySignup
                {
                    Activity = activity,
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
                    .Select(taskSkill => new UserSkill() { SkillId = taskSkill.SkillId, UserId = user.Id });

                user.AssociatedSkills.AddRange(skillsToAdd.Where(skillToAdd => user.AssociatedSkills.All(userSkill => userSkill.SkillId != skillToAdd.SkillId)));

                _context.Update(user);
            }

            await _context.SaveChangesAsync();

            //Notify admins of a new volunteer
            await _bus.PublishAsync(new VolunteerSignupNotification() { ActivityId = model.ActivityId, UserId = model.UserId, TaskId = task.Id });

            return new TaskSignupResult {Status = "success", Task = task};
        }
    }
}
