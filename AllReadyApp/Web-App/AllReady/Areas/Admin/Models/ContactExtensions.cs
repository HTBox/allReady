
using AllReady.Models;
using System.Collections.Generic;
using System.Linq;

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
            if (campaign.CampaignContacts == null)
            {
                campaign.CampaignContacts = new List<CampaignContact>();
            }
            var primaryCampaignContact = campaign.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
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
                _context.CampaignContacts.Remove(primaryCampaignContact);
                _context.Remove(primaryCampaignContact);//Not Needed?
                _context.Remove(primaryCampaignContact.Contact);

            }
            else
            {
                primaryContact.Email = contactModel.PrimaryContactEmail;
                primaryContact.FirstName = contactModel.PrimaryContactFirstName;
                primaryContact.LastName = contactModel.PrimaryContactLastName;
                primaryContact.PhoneNumber = contactModel.PrimaryContactPhoneNumber;
                _context.Update(primaryContact);

                if (primaryCampaignContact == null)
                {
                    primaryCampaignContact = new CampaignContact
                    {
                        Contact = primaryContact,
                        Campaign = campaign,
                        ContactType = (int)ContactTypes.Primary
                    };
                    campaign.CampaignContacts.Add(primaryCampaignContact);
                    _context.Update(primaryCampaignContact);
                }
            }
            return campaign;
        }
        public static Tenant UpdateTenantContact(this Tenant tenant, IPrimaryContactModel contactModel, AllReadyContext _context)
        {
            if (tenant.TenantContacts == null)
            {
                tenant.TenantContacts = new List<TenantContact>();
            }
            var primaryCampaignContact = tenant.TenantContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
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
                _context.TenantContacts.Remove(primaryCampaignContact);
                _context.Remove(primaryCampaignContact);//Not Needed?
                _context.Remove(primaryCampaignContact.Contact);

            }
            else
            {
                primaryContact.Email = contactModel.PrimaryContactEmail;
                primaryContact.FirstName = contactModel.PrimaryContactFirstName;
                primaryContact.LastName = contactModel.PrimaryContactLastName;
                primaryContact.PhoneNumber = contactModel.PrimaryContactPhoneNumber;
                _context.Update(primaryContact);

                if (primaryCampaignContact == null)
                {
                    primaryCampaignContact = new TenantContact
                    {
                        Contact = primaryContact,
                        Tenant = tenant,
                        ContactType = (int)ContactTypes.Primary
                    };
                    tenant.TenantContacts.Add(primaryCampaignContact);
                    _context.Update(primaryCampaignContact);
                }
            }
            return tenant;
        }

    }
}
