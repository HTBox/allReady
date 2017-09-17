using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Campaigns
{
    public class CampaignByCampaignIdQueryHandler : IAsyncRequestHandler<CampaignByCampaignIdQuery, Campaign>
    {
        private readonly AllReadyContext _context;

        public CampaignByCampaignIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<Campaign> Handle(CampaignByCampaignIdQuery message)
        {
            return await _context.Campaigns
                .Include(x => x.ManagingOrganization)
                .Include(x => x.CampaignGoals)
                .Include(x => x.Events)
                .ThenInclude(a => a.Location)
                .Include(x => x.Location)
                .Include(x => x.ParticipatingOrganizations)
                .SingleOrDefaultAsync(x => x.Published && x.Id == message.CampaignId);
        }
    }
}