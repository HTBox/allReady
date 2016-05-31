using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignDetailQueryHandler : IAsyncRequestHandler<CampaignDetailQuery, CampaignDetailModel>
    {
        private AllReadyContext _context;

        public CampaignDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<CampaignDetailModel> Handle(CampaignDetailQuery message)
        {
            var campaign = await _context.Campaigns
                .AsNoTracking()
                .Include(c => c.Events)
                .Include(m => m.ManagingOrganization)
                .Include(ci => ci.CampaignImpact)
                .Include(c => c.CampaignContacts).ThenInclude(c => c.Contact)
                .Include(l => l.Location)
                .SingleOrDefaultAsync(c => c.Id == message.CampaignId)
                .ConfigureAwait(false);

            CampaignDetailModel result = null;

            if (campaign != null)
            {
                result = new CampaignDetailModel
                {
                    Id = campaign.Id,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    ExternalUrl = campaign.ExternalUrl,
                    ExternalUrlText = campaign.ExternalUrlText,
                    OrganizationId = campaign.ManagingOrganizationId,
                    OrganizationName = campaign.ManagingOrganization.Name,
                    ImageUrl = campaign.ImageUrl,
                    TimeZoneId = campaign.TimeZoneId,
                    StartDate = campaign.StartDateTime,
                    EndDate = campaign.EndDateTime,
                    CampaignImpact = campaign.CampaignImpact,
                    Location = campaign.Location.ToModel(),
                    Locked = campaign.Locked,
                    Featured = campaign.Featured,
                    Events = campaign.Events.Select(a => new EventSummaryModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        TimeZoneId = campaign.TimeZoneId,
                        StartDateTime = a.StartDateTime,
                        EndDateTime = a.EndDateTime,
                        CampaignId = campaign.Id,
                        CampaignName = campaign.Name,
                        OrganizationId = campaign.ManagingOrganizationId,
                        OrganizationName = campaign.ManagingOrganization.Name,
                        ImageUrl = a.ImageUrl,
                        UsersSignedUp = a.UsersSignedUp,
                        IsLimitVolunteers = a.IsLimitVolunteers,
                        IsAllowWaitList = a.IsAllowWaitList
                    })
                };

                if (!campaign.CampaignContacts.Any())// Include isn't including
                {
                    campaign.CampaignContacts = await _context.CampaignContacts.Include(c => c.Contact).Where(cc => cc.CampaignId == campaign.Id).ToListAsync().ConfigureAwait(false);
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