using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Extensions;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandler : IAsyncRequestHandler<EditCampaignCommand, int>
    {
        private readonly AllReadyContext _context;

        public EditCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditCampaignCommand message)
        {
            var campaign = await _context.Campaigns
                .Include(l => l.Location)
                .Include(tc => tc.CampaignContacts)
                .Include(i => i.CampaignGoals)
                .SingleOrDefaultAsync(c => c.Id == message.Campaign.Id);

            if (campaign == null)
            {
                campaign = new Campaign();
                _context.Campaigns.Add(campaign);
            }

            campaign.Name = message.Campaign.Name;
            campaign.Description = message.Campaign.Description;
            campaign.FullDescription = message.Campaign.FullDescription;
            campaign.ExternalUrl = message.Campaign.ExternalUrl;
            campaign.ExternalUrlText = message.Campaign.ExternalUrlText;

            campaign.TimeZoneId = message.Campaign.TimeZoneId;
            campaign.StartDateTime = message.Campaign.StartDate;
            campaign.EndDateTime = message.Campaign.EndDate.AddDays(1).AddSeconds(-1); //Adjusted to the end of the day

            campaign.ManagingOrganizationId = message.Campaign.OrganizationId;
            campaign.ImageUrl = message.Campaign.ImageUrl;

            CreateUpdateOrDeleteCampaignPrimaryContact(campaign, message.Campaign);
            campaign.Location = campaign.Location.UpdateModel(message.Campaign.Location);

            campaign.Featured = message.Campaign.Featured;
            campaign.Published = message.Campaign.Published;
            campaign.Headline = message.Campaign.Headline;
            
            await _context.SaveChangesAsync();

            return campaign.Id;
        }

        private void CreateUpdateOrDeleteCampaignPrimaryContact(Campaign campaign, IPrimaryContactViewModel contactModel)
        {
            var hasPrimaryContact = campaign.CampaignContacts != null &&
                campaign.CampaignContacts.Any(campaignContact => campaignContact.ContactType == (int)ContactTypes.Primary);

            var addOrUpdatePrimaryContact = !string.IsNullOrWhiteSpace(string.Concat(contactModel.PrimaryContactEmail?.Trim() + contactModel.PrimaryContactFirstName?.Trim(), contactModel.PrimaryContactLastName?.Trim(), contactModel.PrimaryContactPhoneNumber?.Trim()));

            // Update existing Primary Campaign Contact
            if (hasPrimaryContact && addOrUpdatePrimaryContact)
            {
                var contactId = campaign.CampaignContacts.Single(campaignContact => campaignContact.ContactType == (int)ContactTypes.Primary).ContactId;
                var contact = _context.Contacts.Single(c => c.Id == contactId);

                contact.Email = contactModel.PrimaryContactEmail;
                contact.FirstName = contactModel.PrimaryContactFirstName;
                contact.LastName = contactModel.PrimaryContactLastName;
                contact.PhoneNumber = contactModel.PrimaryContactPhoneNumber;
            }
            // Delete existing Primary Campaign Contact
            else if (hasPrimaryContact && !addOrUpdatePrimaryContact)
            {
                var campaignContact = campaign.CampaignContacts.Single(cc => cc.ContactType == (int)ContactTypes.Primary);
                var contact = _context.Contacts.Single(c => c.Id == campaignContact.ContactId);
                _context.Remove(contact);
                _context.Remove(campaignContact);
            }
            // Add a Primary Campaign Contact
            else if (!hasPrimaryContact && addOrUpdatePrimaryContact)
            {
                var campaignContact = new CampaignContact()
                {
                    ContactType = (int)ContactTypes.Primary,
                    Contact = new Contact()
                    {
                        Email = contactModel.PrimaryContactEmail,
                        FirstName = contactModel.PrimaryContactFirstName,
                        LastName = contactModel.PrimaryContactLastName,
                        PhoneNumber = contactModel.PrimaryContactPhoneNumber
                    },
                };

                if (campaign.CampaignContacts == null) campaign.CampaignContacts = new List<CampaignContact>();
                campaign.CampaignContacts.Add(campaignContact);
            }
        }
    }
}
