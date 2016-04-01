using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

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
                .Include(mt => mt.ManagingOrganization)
                .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                .Include(c => c.CampaignContacts).ThenInclude(tc => tc.Contact)
                .SingleOrDefault(c => c.Id == message.CampaignId);

            if (campaign != null)
            {
                result = new CampaignSummaryModel
                {
                    Id = campaign.Id,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    FullDescription = campaign.FullDescription,
                    OrganizationId = campaign.ManagingOrganizationId,
                    OrganizationName = campaign.ManagingOrganization.Name,
                    ImageUrl = campaign.ImageUrl,
                    TimeZoneId = campaign.TimeZoneId,
                    StartDate = campaign.StartDateTime,
                    EndDate = campaign.EndDateTime,
                    Location = campaign.Location.ToEditModel(),
                    CampaignImpact = campaign.CampaignImpact != null ? campaign.CampaignImpact : new CampaignImpact()
                };

                if (!campaign.CampaignContacts.Any())// Include isn't including
                {
                    campaign.CampaignContacts = _context.CampaignContacts.Include(c => c.Contact).Where(cc => cc.CampaignId == campaign.Id).ToList();
                }

                if (campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
                {
                    result = (CampaignSummaryModel)campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(result);
                }
            }

            return result;
        }
    }
}