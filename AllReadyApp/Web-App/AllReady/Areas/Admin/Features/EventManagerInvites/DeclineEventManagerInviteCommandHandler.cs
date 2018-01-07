using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class DeclineEventManagerInviteCommandHandler : AsyncRequestHandler<DeclineEventManagerInviteCommand>
    {
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext _context;

        public DeclineEventManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeclineEventManagerInviteCommand message)
        {
            var invite = _context.EventManagerInvites.Single(i => i.Id == message.EventManagerInviteId);
            invite.RejectedDateTimeUtc = DateTimeUtcNow();
            await _context.SaveChangesAsync();
        }
    }
}
