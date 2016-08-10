using AllReady.Models;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AllReady.ViewModels.Home;

namespace AllReady.Features.Campaigns
{
    public class FeaturedCampaignQueryHandlerAsync : IAsyncRequestHandler<FeaturedCampaignQueryAsync, CampaignSummaryViewModel>
    {
        private readonly AllReadyContext _context;

        public FeaturedCampaignQueryHandlerAsync(AllReadyContext dataAccess)
        {
            _context = dataAccess;
        }

        public async Task<CampaignSummaryViewModel> Handle(FeaturedCampaignQueryAsync message)
        {
            return await _context.Campaigns.AsNoTracking()
                .Where(c => c.Featured)
                .Include(c => c.ManagingOrganization)
                .Select(c => new CampaignSummaryViewModel
                {
                    Id = c.Id,
                    Title = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    OrganizationName = c.ManagingOrganization.Name,
                    Headline = c.Headline
                })
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();
        }
    }
}