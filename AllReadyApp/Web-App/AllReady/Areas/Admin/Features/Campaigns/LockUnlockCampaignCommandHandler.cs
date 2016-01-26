using AllReady.Models;
using MediatR;
using System.Linq;

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
                if (campaign.Locked)
                {
                    campaign.Locked = false;
                }
                else
                {
                    campaign.Locked = true;
                }

                _context.Update(campaign);
                _context.SaveChanges();
            }
        }
    }
}