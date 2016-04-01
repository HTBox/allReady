using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Activity
{
    public class ActivitySignupHandler : AsyncRequestHandler<ActivitySignupCommand>
    {
        private readonly IMediator _mediator;
        private readonly AllReadyContext _context;

        public ActivitySignupHandler(IMediator mediator, AllReadyContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        protected override async Task HandleCore(ActivitySignupCommand message)
        {
            var activitySignup = message.ActivitySignup;

            var user = await _context.Users
                .Include(u => u.AssociatedSkills)
                .SingleOrDefaultAsync(u => u.Id == activitySignup.UserId).ConfigureAwait(false);

            var activity = await _context.Activities
                .Include(a => a.RequiredSkills)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .SingleOrDefaultAsync(a => a.Id == activitySignup.ActivityId).ConfigureAwait(false);

            activity.UsersSignedUp = activity.UsersSignedUp ?? new List<ActivitySignup>();

            // If the user is already signed up for some reason, stop don't signup again, please
            if (!activity.UsersSignedUp.Any(acsu => acsu.User.Id == user.Id))
            {
                activity.UsersSignedUp.Add(new ActivitySignup
                {
                    Activity = activity,
                    User = user,
                    PreferredEmail = activitySignup.PreferredEmail,
                    PreferredPhoneNumber = activitySignup.PreferredPhoneNumber,
                    AdditionalInfo = activitySignup.AdditionalInfo,
                    SignupDateTime = DateTime.UtcNow
                });

                _context.Update(activity);

                //Add selected new skills (if any) to the current user
                if (activitySignup.AddSkillIds.Count > 0)
                {
                    var skillsToAdd = activity.RequiredSkills
                        .Where(acsk => activitySignup.AddSkillIds.Contains(acsk.SkillId))
                        .Select(acsk => new UserSkill() { SkillId = acsk.SkillId, UserId = user.Id });
                    user.AssociatedSkills.AddRange(skillsToAdd.Where(toAdd => !user.AssociatedSkills.Any(existing => existing.SkillId == toAdd.SkillId)));

                    _context.Update(user);
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);

                //Notify admins of a new volunteer
                await _mediator.PublishAsync(new VolunteerSignupNotification() { ActivityId = activitySignup.ActivityId, UserId = activitySignup.UserId, TaskId = null })
                    .ConfigureAwait(false);
            }
        }
    }
}