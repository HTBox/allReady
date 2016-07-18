using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public static class ContactExtensions
    {       
        public static IPrimaryContactModel ToEditModel(this Contact contact, IPrimaryContactModel contactModel)
        {
            if (contact != null)
            {
                contactModel.PrimaryContactEmail = contact.Email;
                contactModel.PrimaryContactFirstName = contact.FirstName;
                contactModel.PrimaryContactLastName = contact.LastName;
                contactModel.PrimaryContactPhoneNumber = contact.PhoneNumber;
            }

            return contactModel;
        }

        public static Campaign UpdateCampaignContact(this Campaign campaign, IPrimaryContactModel contactModel, AllReadyContext _context)
        {
            bool hasPrimaryContact = campaign.CampaignContacts != null &&
                campaign.CampaignContacts.Any(campaignContact => campaignContact.ContactType == (int)ContactTypes.Primary);

            bool addOrUpdatePrimaryContact = !string.IsNullOrWhiteSpace(string.Concat(contactModel.PrimaryContactEmail?.Trim() + contactModel.PrimaryContactFirstName?.Trim(), contactModel.PrimaryContactLastName?.Trim(), contactModel.PrimaryContactPhoneNumber?.Trim()));

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
                _context.Add(campaignContact);
            }
            return campaign;
        }

        public static Organization UpdateOrganizationContact(this Organization organization, IPrimaryContactModel contactModel, AllReadyContext _context)
        {
            if (organization.OrganizationContacts == null)
            {
                organization.OrganizationContacts = new List<OrganizationContact>();
            }
            var primaryCampaignContact = organization.OrganizationContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
            var contactId = primaryCampaignContact?.ContactId;
            Contact primaryContact;

            var contactInfo = string.Concat(contactModel.PrimaryContactEmail?.Trim() + contactModel.PrimaryContactFirstName?.Trim(), contactModel.PrimaryContactLastName?.Trim(), contactModel.PrimaryContactPhoneNumber?.Trim());
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
                _context.OrganizationContacts.Remove(primaryCampaignContact);
                _context.Remove(primaryCampaignContact);//Not Needed?
                _context.Remove(primaryCampaignContact.Contact);

            }
            else
            {
                primaryContact.Email = contactModel.PrimaryContactEmail;
                primaryContact.FirstName = contactModel.PrimaryContactFirstName;
                primaryContact.LastName = contactModel.PrimaryContactLastName;
                primaryContact.PhoneNumber = contactModel.PrimaryContactPhoneNumber;
                _context.AddOrUpdate(primaryContact);

                if (primaryCampaignContact == null)
                {
                    primaryCampaignContact = new OrganizationContact
                    {
                        Contact = primaryContact,
                        Organization = organization,
                        ContactType = (int)ContactTypes.Primary
                    };
                    organization.OrganizationContacts.Add(primaryCampaignContact);
                    _context.AddOrUpdate(primaryCampaignContact);
                }
            }

            return organization;
        }
    }
}
