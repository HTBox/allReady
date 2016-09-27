using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommandHandlerAsync : AsyncRequestHandler<LockUnlockCampaignCommandAsync>
    {
        private AllReadyContext _context;

        public LockUnlockCampaignCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;

        }
        protected override async Task HandleCore(LockUnlockCampaignCommandAsync message)
        {
            var campaign = await _context.Campaigns
                .SingleOrDefaultAsync(c => c.Id == message.CampaignId)
                .ConfigureAwait(false);

            if (campaign != null)
            {
                campaign.Locked = !campaign.Locked;
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}