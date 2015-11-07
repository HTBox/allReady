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
    public class CampaignListQueryHandler : IRequestHandler<CampaignListQuery, IEnumerable<CampaignSummaryViewModel>>
    {
        private IAllReadyContext _context;

        public CampaignListQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public IEnumerable<CampaignSummaryViewModel> Handle(CampaignListQuery message)
        {
            var campaigns = _context.Campaigns
                //.Where(c => c.ManagingTenantId == message.TenantId)
                .Select(c => new CampaignSummaryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    TenantId = c.ManagingTenantId,
                    TenantName = c.ManagingTenant.Name,
                    StartDate = c.StartDateTimeUtc,
                    EndDate = c.EndDateTimeUtc
                });

            return campaigns;
        }
    }
}
