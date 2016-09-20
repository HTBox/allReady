using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Extensions;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQueryHandlerAsync : IAsyncRequestHandler<CampaignSummaryQueryAsync, CampaignSummaryViewModel>
    {
        private AllReadyContext _context;

        public CampaignSummaryQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;

        }

        public async Task<CampaignSummaryViewModel> Handle(CampaignSummaryQueryAsync message)
        {
            CampaignSummaryViewModel result = null;

            var campaign = await _context.Campaigns
                .AsNoTracking()
                .Include(ci => ci.CampaignImpact)
                .Include(mt => mt.ManagingOrganization)
                .Include(l => l.Location)
                .Include(c => c.CampaignContacts).ThenInclude(tc => tc.Contact)
                .SingleOrDefaultAsync(c => c.Id == message.CampaignId)
                .ConfigureAwait(false);

            if (campaign != null)
            {
                result = new CampaignSummaryViewModel
                {
                    Id = campaign.Id,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    Featured = campaign.Featured,
                    FullDescription = campaign.FullDescription,
                    ExternalUrl = campaign.ExternalUrl,
                    ExternalUrlText = campaign.ExternalUrlText,
                    OrganizationId = campaign.ManagingOrganizationId,
                    OrganizationName = campaign.ManagingOrganization.Name,
                    ImageUrl = campaign.ImageUrl,
                    TimeZoneId = campaign.TimeZoneId,
                    StartDate = campaign.StartDateTime,
                    EndDate = campaign.EndDateTime,
                    Location = campaign.Location.ToEditModel(),
                    CampaignImpact = campaign.CampaignImpact ?? new CampaignImpact(),
                    Headline = campaign.Headline
                };

                if (!campaign.CampaignContacts.Any())// Include isn't including
                {
                    campaign.CampaignContacts = await _context.CampaignContacts.Include(c => c.Contact).Where(cc => cc.CampaignId == campaign.Id).ToListAsync().ConfigureAwait(false);
                }

                if (campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
                {
                    result = (CampaignSummaryViewModel)campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(result);
                }
            }

            return result;
        }
    }
}