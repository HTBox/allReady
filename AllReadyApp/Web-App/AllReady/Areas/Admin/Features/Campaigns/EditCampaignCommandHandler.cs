using AllReady.Areas.Admin.Models;
using Microsoft.Data.Entity;
using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandler : IRequestHandler<EditCampaignCommand, int>
    {
        private AllReadyContext _context;

        public EditCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        public int Handle(EditCampaignCommand message)
        {
            var campaign = _context.Campaigns
                                    .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                    .Include(tc => tc.CampaignContacts)

                .SingleOrDefault(c => c.Id == message.Campaign.Id);

            if (campaign == null)
            {
                campaign = new Campaign();
            }

            campaign.Name = message.Campaign.Name;
            campaign.Description = message.Campaign.Description;
            campaign.StartDateTimeUtc = message.Campaign.StartDate;
            campaign.EndDateTimeUtc = message.Campaign.EndDate;
            campaign.ManagingTenantId = message.Campaign.TenantId;



            var contactInfo = string.Concat(message.Campaign.PrimaryContactEmail?.Trim() + message.Campaign.PrimaryContactFirstName?.Trim(), message.Campaign.PrimaryContactLastName?.Trim(), message.Campaign.PrimaryContactPhoneNumber?.Trim());
            var primaryCampaignContact = campaign.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactType.Primary);
            var contactId = primaryCampaignContact?.ContactId;
            Contact primaryContact;

            if (contactId == null)
            {
                primaryContact = new Contact();
            }
            else
            {
                primaryContact = _context.Contacts.Single(c => c.Id == contactId);
            }
            if (string.IsNullOrWhiteSpace(contactInfo) && primaryCampaignContact != null)
            {
                //Delete
                _context.CampaignContacts.Remove(primaryCampaignContact);
                _context.Remove(primaryCampaignContact);
                _context.Remove(primaryCampaignContact.Contact);

            }
            else
            {
                primaryContact.Email = message.Campaign.PrimaryContactEmail;
                primaryContact.FirstName = message.Campaign.PrimaryContactFirstName;
                primaryContact.LastName = message.Campaign.PrimaryContactLastName;
                primaryContact.PhoneNumber = message.Campaign.PrimaryContactPhoneNumber;
                _context.Update(primaryContact);

                if (primaryCampaignContact == null)
                {
                    primaryCampaignContact = new CampaignContact
                    {
                        Contact = primaryContact,
                        Campaign = campaign,
                        ContactType = (int)ContactType.Primary
                    };
                    campaign.CampaignContacts.Add(primaryCampaignContact);
                    _context.Update(primaryCampaignContact);
                }

            }
            if (message.Campaign.Location != null)
            {
                var locationInfo = string.Concat(message.Campaign.Location?.Address1?.Trim(), message.Campaign.Location?.Address2?.Trim(), message.Campaign.Location?.City?.Trim(), message.Campaign.Location?.State?.Trim(), message.Campaign.Location?.PostalCode?.Trim(), message.Campaign.Location?.Name?.Trim(), message.Campaign.Location?.PhoneNumber?.Trim());
                if (string.IsNullOrWhiteSpace(locationInfo))
                {
                    campaign.Location = null;
                }
                else
                {
                    if (campaign.Location == null || message.Campaign.Location.Id.GetValueOrDefault() != 0)
                    {
                        campaign.Location = new Location();
                    }
                    campaign.Location = ToModel(campaign.Location, message.Campaign.Location);
                }
                _context.Update(campaign.Location);
            }


            _context.Update(campaign);
            _context.SaveChanges();
            return campaign.Id;
        }

        private Location ToModel(Location location, LocationEditModel locationEditModel)
        {
            location.Address1 = locationEditModel.Address1;
            location.Address2 = locationEditModel.Address2;
            location.City = locationEditModel.City;
            location.Country = locationEditModel.Country;
            location.Name = locationEditModel.Name;
            location.PhoneNumber = locationEditModel.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(locationEditModel.PostalCode))
            {
                location.PostalCode = new PostalCodeGeo { PostalCode = locationEditModel.PostalCode, City = locationEditModel.City, State = locationEditModel.State };
            }
            location.State = locationEditModel.State;
            return location;
        }
    }
}
