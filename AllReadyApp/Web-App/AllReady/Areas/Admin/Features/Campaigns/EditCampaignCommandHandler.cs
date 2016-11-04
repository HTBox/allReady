﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Providers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandler : IAsyncRequestHandler<EditCampaignCommand, int>
    {
        private readonly AllReadyContext _context;
        private readonly IConvertDateTimeOffset dateTimeOffsetsConverter;

        public EditCampaignCommandHandler(AllReadyContext context, IConvertDateTimeOffset dateTimeOffsetsConverter)
        {
            _context = context;
            this.dateTimeOffsetsConverter = dateTimeOffsetsConverter;
        }

        public async Task<int> Handle(EditCampaignCommand message)
        {
            var campaign = await _context.Campaigns
                .Include(l => l.Location)
                .Include(tc => tc.CampaignContacts)
                .Include(i => i.CampaignImpact)
                .SingleOrDefaultAsync(c => c.Id == message.Campaign.Id)
                .ConfigureAwait(false) ?? new Campaign();

            campaign.Name = message.Campaign.Name;
            campaign.Description = message.Campaign.Description;
            campaign.FullDescription = message.Campaign.FullDescription;
            campaign.ExternalUrl = message.Campaign.ExternalUrl;
            campaign.ExternalUrlText = message.Campaign.ExternalUrlText;

            campaign.TimeZoneId = message.Campaign.TimeZoneId;
            campaign.StartDateTime = dateTimeOffsetsConverter.ConvertDateTimeOffsetTo(campaign.TimeZoneId, message.Campaign.StartDate);
            campaign.EndDateTime = dateTimeOffsetsConverter.ConvertDateTimeOffsetTo(campaign.TimeZoneId, message.Campaign.EndDate, 23, 59, 59);

            campaign.ManagingOrganizationId = message.Campaign.OrganizationId;
            campaign.ImageUrl = message.Campaign.ImageUrl;

            CreateUpdateOrDeleteCampaignPrimaryContact(campaign, message.Campaign);
            campaign.CampaignImpact = campaign.CampaignImpact.UpdateModel(message.Campaign.CampaignImpact);
            campaign.Location = campaign.Location.UpdateModel(message.Campaign.Location);

            campaign.Featured = message.Campaign.Featured;
            campaign.Headline = message.Campaign.Headline;

            _context.AddOrUpdate(campaign);

            await _context.SaveChangesAsync().ConfigureAwait(false);

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