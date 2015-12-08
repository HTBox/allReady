using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignListQueryHandler : IRequestHandler<CampaignListQuery, IEnumerable<CampaignSummaryModel>>
    {
        private AllReadyContext _context;

        public CampaignListQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public IEnumerable<CampaignSummaryModel> Handle(CampaignListQuery message)
        {
            var campaigns = _context.Campaigns
                //.Where(c => c.ManagingTenantId == message.TenantId)
                .Select(c => new CampaignSummaryModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    TenantId = c.ManagingOrganizationId,
                    TenantName = c.ManagingOrganization.Name,
                    TimeZoneId = c.TimeZoneId,
                    StartDate = c.StartDateTime,
                    EndDate = c.EndDateTime
                });

            return campaigns;
        }
    }
}
