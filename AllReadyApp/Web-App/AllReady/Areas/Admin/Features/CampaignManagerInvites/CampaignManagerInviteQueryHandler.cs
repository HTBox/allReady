using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CampaignManagerInviteQueryHandler : IAsyncRequestHandler<CampaignManagerInviteQuery, CampaignManagerInviteViewModel>
    {
        private AllReadyContext _context;

        public CampaignManagerInviteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<CampaignManagerInviteViewModel> Handle(CampaignManagerInviteQuery message)
        {
            var campaign = await _context.Campaigns.AsNoTracking().SingleAsync(c => c.Id == message.CampaignId);

            return new CampaignManagerInviteViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                OrganizationId = campaign.ManagingOrganizationId
            };
        }
    }
}
