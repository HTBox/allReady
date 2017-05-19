using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Campaigns
{
    public class AuthorizedCampaignsQueryHandler : IAsyncRequestHandler<AuthorizedCampaignsQuery, List<ManageCampaignViewModel>>
    {
        private readonly AllReadyContext _context;

        public AuthorizedCampaignsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<ManageCampaignViewModel>> Handle(AuthorizedCampaignsQuery message)
        {
            return await _context.CampaignManagers.Where(c => c.UserId == message.UserId)
                .Select(c => c.Campaign)
                .Where(c => !c.Locked && c.Published)
                .Select(campaign => new ManageCampaignViewModel
                {
                    Id = campaign.Id,
                    Description = campaign.Description,
                    EndDate = campaign.EndDateTime,
                    Featured = campaign.Featured,
                    ManagingOrganizationLogo = campaign.ManagingOrganization.LogoUrl,
                    ManagingOrganizationName = campaign.ManagingOrganization.Name,
                    Name = campaign.Name,
                    StartDate = campaign.StartDateTime
                })
                .ToListAsync();
        }
    }
}