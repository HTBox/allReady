using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommandHandler : AsyncRequestHandler<LockUnlockCampaignCommand>
    {
        private AllReadyContext _context;

        public LockUnlockCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        protected override async Task HandleCore(LockUnlockCampaignCommand message)
        {
            var campaign = await _context.Campaigns.SingleOrDefaultAsync(c => c.Id == message.CampaignId).ConfigureAwait(false);

            if (campaign != null)
            {
                campaign.Locked = !campaign.Locked;

                _context.Update(campaign);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}