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
                result = new CampaignDetailModel()
                {
                    Id = campaign.Id,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    TenantId = campaign.ManagingTenantId,
                    TenantName = campaign.ManagingTenant.Name,
                    ImageUrl = campaign.ImageUrl,
                    StartDate = campaign.StartDateTimeUtc,
                    EndDate = campaign.EndDateTimeUtc,
                    CampaignImpact = campaign.CampaignImpact,
                    Location = ToModel(campaign.Location),
                    Activities = campaign.Activities.Select(a => new ActivitySummaryModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        StartDateTime = a.StartDateTimeUtc,
                        EndDateTime = a.EndDateTimeUtc,
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

                var contact = campaign.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactType.Primary)?.Contact;
                if (contact != null)
                {
                    //var contact = _context.Contacts.Single(c => c.Id == contactId);
                    result.PrimaryContactEmail = contact.Email;
                    result.PrimaryContactFirstName = contact.FirstName;
                    result.PrimaryContactLastName = contact.LastName;
                    result.PrimaryContactPhoneNumber = contact.PhoneNumber;
                }

            }
            return result;
        }
        private LocationDisplayModel ToModel(Location location)
        {
            if (location == null) { return null; }
            return new LocationDisplayModel
            {
                Id = location.Id,
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                Country = location.Country,
                Name = location.Name,
                PhoneNumber = location.PhoneNumber,
                PostalCode = location.PostalCode?.PostalCode,
                State = location.State
            };

        }
    }
}
