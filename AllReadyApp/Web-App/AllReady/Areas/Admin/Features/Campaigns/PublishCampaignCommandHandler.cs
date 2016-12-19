using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class PublishCampaignCommandHandler : AsyncRequestHandler<PublishCampaignCommand>
    {
        private AllReadyContext _context;

        public PublishCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(PublishCampaignCommand message)
        {
            var campaign = await _context.Campaigns.SingleOrDefaultAsync(c => c.Id == message.CampaignId);
            if (campaign != null)
            {
                campaign.Published = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}