using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerInviteCommandHandler : IAsyncRequestHandler<CreateEventManagerInviteCommand, int>
    {
        private AllReadyContext _context;

        public CreateEventManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public async Task<int> Handle(CreateEventManagerInviteCommand message)
        {
            var eventManagerInvite = new EventManagerInvite
            {
                InviteeEmailAddress = message.Invite.InviteeEmailAddress,
                SentDateTimeUtc = DateTimeUtcNow(),
                CustomMessage = message.Invite.CustomMessage,
                SenderUserId = message.UserId,
                EventId = message.Invite.EventId
            };
            _context.EventManagerInvites.Add(eventManagerInvite);
            await _context.SaveChangesAsync();
            return eventManagerInvite.Id;
        }

    }
}
