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
                .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                .Include(c => c.CampaignContacts).ThenInclude(tc => tc.Contact)
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
                    Location = ToModel(campaign.Location),
                    CampaignImpact = campaign.CampaignImpact != null ? campaign.CampaignImpact : new CampaignImpact()
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
        private LocationEditModel ToModel(Location location)
        {
            if (location == null) { return null; }
            return new LocationEditModel
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