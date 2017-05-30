using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Invite
{
    public class CreateEventManagerInviteCommandHandler : AsyncRequestHandler<CreateEventManagerInviteCommand>
    {
        private AllReadyContext _context;

        public CreateEventManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        protected override async Task HandleCore(CreateEventManagerInviteCommand message)
        {
            var @event = await _context.Events.AsNoTracking().SingleOrDefaultAsync(e => e.Id == message.Invite.EventId);

            if (@event == null) throw new ArgumentException("EventId cannot be null for Event manager invite");

            var eventManagerInvite = new EventManagerInvite
            {
                InviteeEmailAddress = message.Invite.InviteeEmailAddress,
                SentDateTimeUtc = DateTimeUtcNow(),
                CustomMessage = message.Invite.CustomMessage,
                SenderUserId = message.UserId,
                Event = @event,
            };
            _context.EventManagerInvites.Add(eventManagerInvite);
            await _context.SaveChangesAsync();
        }

    }
}
