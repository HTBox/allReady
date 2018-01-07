using AllReady.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class DeclineCampaignManagerInviteCommandHandler : AsyncRequestHandler<DeclineCampaignManagerInviteCommand>
    {
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext _context;

        public DeclineCampaignManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeclineCampaignManagerInviteCommand message)
        {
            var invite = _context.CampaignManagerInvites.Single(i => i.Id == message.CampaignManagerInviteId);
            invite.RejectedDateTimeUtc = DateTimeUtcNow();
            await _context.SaveChangesAsync();
        }
    }
}
