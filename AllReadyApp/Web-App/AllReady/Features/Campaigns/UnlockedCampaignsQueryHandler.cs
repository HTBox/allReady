using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Campaigns
{
    public class UnlockedCampaignsQueryHandler : IAsyncRequestHandler<UnlockedCampaignsQuery, List<CampaignViewModel>>
    {
        private readonly AllReadyContext _context;

        public UnlockedCampaignsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<CampaignViewModel>> Handle(UnlockedCampaignsQuery message)
        {
            return await _context.Campaigns
                .Include(x => x.ManagingOrganization)
                .Include(x => x.Events)
                .Include(x => x.ParticipatingOrganizations)
                .Where(c => !c.Locked && c.Published)
                .Select(campaign => campaign.ToViewModel()).ToListAsync();
        }
    }
}
