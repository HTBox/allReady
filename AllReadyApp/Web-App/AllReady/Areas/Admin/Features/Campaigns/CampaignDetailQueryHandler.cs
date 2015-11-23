using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignDetailQueryHandler : IRequestHandler<CampaignDetailQuery, CampaignDetailModel>
    {
        private AllReadyContext _context;

        public CampaignDetailQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public CampaignDetailModel Handle(CampaignDetailQuery message)
        {
            var campaign = _context.Campaigns
                                  .AsNoTracking()
                                  .Include(c => c.Activities)
                                  .Include(m => m.ManagingTenant)
                                  .Include(ci => ci.CampaignImpact)
                                  .Include(c => c.CampaignContacts).ThenInclude(c => c.Contact)
                                  .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                                  .SingleOrDefault(c => c.Id == message.CampaignId);

            CampaignDetailModel result = null;

            if (campaign != null)
            {
                result = new CampaignDetailModel
                {
                    Id = campaign.Id,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    TenantId = campaign.ManagingTenantId,
                    TenantName = campaign.ManagingTenant.Name,
                    ImageUrl = campaign.ImageUrl,
                    TimeZoneId = campaign.TimeZoneId,
                    StartDate = campaign.StartDateTime,
                    EndDate = campaign.EndDateTime,
                    CampaignImpact = campaign.CampaignImpact,
                    Location = campaign.Location.ToModel(),
                    Activities = campaign.Activities.Select(a => new ActivitySummaryModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        StartDateTime = a.StartDateTime,
                        EndDateTime = a.EndDateTime,
                        CampaignId = campaign.Id,
                        CampaignName = campaign.Name,
                        TenantId = campaign.ManagingTenantId,
                        TenantName = campaign.ManagingTenant.Name,
                        ImageUrl = a.ImageUrl
                    })
                };
                if (!campaign.CampaignContacts.Any())// Include isn't including
                {
                    campaign.CampaignContacts = _context.CampaignContacts.Include(c => c.Contact).Where(cc => cc.CampaignId == campaign.Id).ToList();
                }
                if (campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
                {
                    result = (CampaignDetailModel)campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(result);
                }
            }
            return result;
        }
         
    }
}
