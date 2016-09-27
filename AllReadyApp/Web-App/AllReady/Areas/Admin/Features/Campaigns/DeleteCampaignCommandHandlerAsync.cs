using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommandHandlerAsync : AsyncRequestHandler<DeleteCampaignCommandAsync>
    {
        private AllReadyContext _context;

        public DeleteCampaignCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteCampaignCommandAsync message)
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