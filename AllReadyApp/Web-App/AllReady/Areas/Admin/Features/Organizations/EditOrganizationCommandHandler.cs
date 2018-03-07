using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Extensions;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class EditOrganizationCommandHandler : IAsyncRequestHandler<EditOrganizationCommand, int>
    {
        private readonly AllReadyContext _context;

        public EditOrganizationCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditOrganizationCommand message)
        {
            var org = await _context.Organizations
                .Include(l => l.Location)
                .Include(tc => tc.OrganizationContacts)
                .SingleOrDefaultAsync(t => t.Id == message.Organization.Id) ?? _context.Add(new Organization()).Entity;

            org.Name = message.Organization.Name;
            org.LogoUrl = message.Organization.LogoUrl;
            org.WebUrl = message.Organization.WebUrl;

            org.DescriptionHtml = message.Organization.Description;
            org.Summary = message.Organization.Summary;

            if (org.OrganizationContacts == null)
            {
                org.OrganizationContacts = new List<OrganizationContact>();
            }

            var primaryCampaignContact = org.OrganizationContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
            var contactId = primaryCampaignContact?.ContactId;
            Contact primaryContact;

            var contactInfo = string.Concat(message.Organization.PrimaryContactEmail?.Trim() + message.Organization.PrimaryContactFirstName?.Trim(), message.Organization.PrimaryContactLastName?.Trim(), message.Organization.PrimaryContactPhoneNumber?.Trim());
            if (contactId == null)
            {
                primaryContact = new Contact();
                _context.Contacts.Add(primaryContact);
            }
            else
            {
                primaryContact = await _context.Contacts.SingleAsync(c => c.Id == contactId);
            }

            if (string.IsNullOrWhiteSpace(contactInfo) && primaryCampaignContact != null)
            {
                //Delete
                _context.OrganizationContacts.Remove(primaryCampaignContact);
                _context.Remove(primaryCampaignContact.Contact);
            }
            else
            {
                primaryContact.Email = message.Organization.PrimaryContactEmail;
                primaryContact.FirstName = message.Organization.PrimaryContactFirstName;
                primaryContact.LastName = message.Organization.PrimaryContactLastName;
                primaryContact.PhoneNumber = message.Organization.PrimaryContactPhoneNumber;
              
                if (primaryCampaignContact == null)
                {
                    primaryCampaignContact = new OrganizationContact
                    {
                        Contact = primaryContact,
                        Organization = org,
                        ContactType = (int)ContactTypes.Primary
                    };
                    org.OrganizationContacts.Add(primaryCampaignContact);
                }
            }
            org.Location = org.Location.UpdateModel(message.Organization.Location);

            org.PrivacyPolicy = message.Organization.PrivacyPolicy;
            org.PrivacyPolicyUrl = message.Organization.PrivacyPolicyUrl;

            await _context.SaveChangesAsync();

            return org.Id;
        }       
    }
}
