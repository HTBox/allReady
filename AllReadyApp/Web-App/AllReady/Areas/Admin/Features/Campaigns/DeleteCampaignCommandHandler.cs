using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommandHandler : AsyncRequestHandler<DeleteCampaignCommand>
    {
        private AllReadyContext _context;

        public DeleteCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteCampaignCommand message)
        {
            var campaign = await _context.Campaigns.SingleOrDefaultAsync(c => c.Id == message.CampaignId).ConfigureAwait(false);
            if (campaign != null)
            {
                _context.Campaigns.Remove(campaign);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}