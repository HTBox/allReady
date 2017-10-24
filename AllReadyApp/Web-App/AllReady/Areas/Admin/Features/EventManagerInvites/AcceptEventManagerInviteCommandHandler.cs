using AllReady.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class AcceptEventManagerInviteCommandHandler : AsyncRequestHandler<AcceptEventManagerInviteCommand>
    {
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext _context;

        public AcceptEventManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(AcceptEventManagerInviteCommand message)
        {
            var invite = _context.EventManagerInvites.Single(i => i.Id == message.EventManagerInviteId);
            invite.AcceptedDateTimeUtc = DateTimeUtcNow();
            await _context.SaveChangesAsync();
        }
    }
}
