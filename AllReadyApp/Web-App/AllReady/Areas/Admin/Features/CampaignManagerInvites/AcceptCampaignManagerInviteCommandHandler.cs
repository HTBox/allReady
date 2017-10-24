using AllReady.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptCampaignManagerInviteCommandHandler : AsyncRequestHandler<AcceptCampaignManagerInviteCommand>
    {
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext _context;

        public AcceptCampaignManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(AcceptCampaignManagerInviteCommand message)
        {
            var invite = _context.CampaignManagerInvites.Single(i => i.Id == message.CampaignManagerInviteId);
            invite.AcceptedDateTimeUtc = DateTimeUtcNow();
            await _context.SaveChangesAsync();
        }
    }
}
