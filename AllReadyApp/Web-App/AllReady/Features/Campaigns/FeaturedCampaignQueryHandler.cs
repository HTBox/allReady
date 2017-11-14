using System;
using AllReady.Models;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AllReady.ViewModels.Home;

namespace AllReady.Features.Campaigns
{
    public class FeaturedCampaignQueryHandler : IAsyncRequestHandler<FeaturedCampaignQuery, CampaignSummaryViewModel>
    {
        private readonly AllReadyContext _context;
        public Func<DateTimeOffset> DateTimeOffsetUtcNow = () => DateTimeOffset.UtcNow;

        public FeaturedCampaignQueryHandler(AllReadyContext dataAccess)
        {
            _context = dataAccess;
        }

        public Task<CampaignSummaryViewModel> Handle(FeaturedCampaignQuery message)
        {
            return _context.Campaigns.AsNoTracking()
                .Where(c => c.EndDateTime.Date >= DateTimeOffsetUtcNow().Date && c.Featured && c.Published)
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
