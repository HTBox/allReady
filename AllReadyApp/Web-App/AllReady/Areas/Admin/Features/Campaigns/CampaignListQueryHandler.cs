using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignListQueryHandler : IRequestHandler<CampaignListQuery, IEnumerable<CampaignSummaryViewModel>>
    {
        private AllReadyContext _context;

        public CampaignListQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<CampaignSummaryViewModel> Handle(CampaignListQuery message)
        {
            var campaignsQuery = _context.Campaigns.Include(c => c.ManagingOrganization).AsNoTracking();

            if (message.OrganizationId.HasValue)
            {
                campaignsQuery = campaignsQuery.Where(c => c.ManagingOrganizationId == message.OrganizationId);
            }

            var campaigns = campaignsQuery.Select(c =>
                    new CampaignSummaryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        OrganizationId = c.ManagingOrganizationId,
                        OrganizationName = c.ManagingOrganization.Name,
                        TimeZoneId = c.TimeZoneId,
                        StartDate = c.StartDateTime,
                        EndDate = c.EndDateTime,
                        Locked = c.Locked,
                        Featured = c.Featured
                    });
            return campaigns;
        }
    }
}
