using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Event
{
    public class EventSignupHandler : AsyncRequestHandler<EventSignupCommand>
    {
        private readonly IMediator _mediator;
        private readonly AllReadyContext _context;

        public EventSignupHandler(IMediator mediator, AllReadyContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        protected override async Task HandleCore(EventSignupCommand message)
        {
            var eventSignup = message.EventSignup;

            var user = await _context.Users
                .Include(u => u.AssociatedSkills)
                .SingleOrDefaultAsync(u => u.Id == eventSignup.UserId).ConfigureAwait(false);

            var campaignEvent = await _context.Events
                .Include(a => a.RequiredSkills)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .SingleOrDefaultAsync(a => a.Id == eventSignup.EventId).ConfigureAwait(false);

            campaignEvent.UsersSignedUp = campaignEvent.UsersSignedUp ?? new List<EventSignup>();

            // If the user is already signed up for some reason, stop don't signup again, please
            if (!campaignEvent.UsersSignedUp.Any(acsu => acsu.User.Id == user.Id))
            {
                campaignEvent.UsersSignedUp.Add(new EventSignup
                {
                    Event = campaignEvent,
                    User = user,
                    PreferredEmail = eventSignup.PreferredEmail,
                    PreferredPhoneNumber = eventSignup.PreferredPhoneNumber,
                    AdditionalInfo = eventSignup.AdditionalInfo,
                    SignupDateTime = DateTime.UtcNow
                });

                _context.Update(campaignEvent);

                //Add selected new skills (if any) to the current user
                if (eventSignup.AddSkillIds.Count > 0)
                {
                    var skillsToAdd = campaignEvent.RequiredSkills
                        .Where(acsk => eventSignup.AddSkillIds.Contains(acsk.SkillId))
                        .Select(acsk => new UserSkill { SkillId = acsk.SkillId, UserId = user.Id });
                    user.AssociatedSkills.AddRange(skillsToAdd.Where(toAdd => !user.AssociatedSkills.Any(existing => existing.SkillId == toAdd.SkillId)));

                    _context.Update(user);
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);

                    //Notify admins of a new volunteer
                await _mediator.PublishAsync(new VolunteerSignupNotification { EventId = eventSignup.EventId, UserId = eventSignup.UserId, TaskId = null })
                    .ConfigureAwait(false);
            }
        }
    }
}
