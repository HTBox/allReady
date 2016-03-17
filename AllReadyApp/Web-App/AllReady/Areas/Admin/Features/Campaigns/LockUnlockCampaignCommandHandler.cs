using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommandHandler : RequestHandler<LockUnlockCampaignCommand>
    {
        private AllReadyContext _context;

        public LockUnlockCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        protected override void HandleCore(LockUnlockCampaignCommand message)
        {
            var campaign = 
                _context.Campaigns.SingleOrDefault(c => c.Id == message.CampaignId);

            if (campaign != null)
            {
                campaign.Locked = !campaign.Locked;

                _context.Update(campaign);
                _context.SaveChanges();
            }
        }
    }
}