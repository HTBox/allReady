using AllReady.Areas.Admin.ViewModels;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignQueryHandler : IRequestHandler<CampaignQuery, CampaignSummaryViewModel>
    {
        private AllReadyContext _context;

        public CampaignQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public CampaignSummaryViewModel Handle(CampaignQuery message)
        {
            var campaign = _context.Campaigns
                .Select(c => new CampaignSummaryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    TenantId = c.ManagingTenantId,
                    TenantName = c.ManagingTenant.Name,
                    ImageUrl = c.ImageUrl,
                    StartDate = c.StartDateTimeUtc,
                    EndDate = c.EndDateTimeUtc
                }).SingleOrDefault(c => c.Id == message.CampaignId);
                    
            return campaign;
        }
    }
}
