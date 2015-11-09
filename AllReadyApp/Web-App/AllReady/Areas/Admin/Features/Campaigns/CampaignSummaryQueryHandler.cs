using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQueryHandler : IRequestHandler<CampaignSummaryQuery, CampaignSummaryModel>
    {
        private AllReadyContext _context;

        public CampaignSummaryQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public CampaignSummaryModel Handle(CampaignSummaryQuery message)
        {
            CampaignSummaryModel result = null;

            var campaign = _context.Campaigns
                .AsNoTracking()
                .Include(ci => ci.CampaignImpact)
                .Include(mt => mt.ManagingTenant)
                .SingleOrDefault(c => c.Id == message.CampaignId);

            if (campaign != null)
            {
                result = new CampaignSummaryModel()
                {
                    Id = campaign.Id,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    FullDescription = campaign.FullDescription,
                    TenantId = campaign.ManagingTenantId,
                    TenantName = campaign.ManagingTenant.Name,
                    ImageUrl = campaign.ImageUrl,
                    StartDate = campaign.StartDateTimeUtc,
                    EndDate = campaign.EndDateTimeUtc,
                    CampaignImpact = campaign.CampaignImpact != null ? campaign.CampaignImpact : new CampaignImpact()
                };
            }
                    
            return result;
        }
    }
}
