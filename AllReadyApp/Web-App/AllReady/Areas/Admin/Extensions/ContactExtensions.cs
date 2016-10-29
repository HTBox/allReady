using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Extensions;
using AllReady.Models;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Extensions
{
    public static class ContactExtensions
    {       
        public static IPrimaryContactViewModel ToEditModel(this Contact contact, IPrimaryContactViewModel contactModel)
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

        public static async Task<Organization> UpdateOrganizationContact(this Organization organization, IPrimaryContactViewModel contactModel, AllReadyContext _context)
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
                primaryContact = await _context.Contacts.SingleAsync(c => c.Id == contactId);
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
