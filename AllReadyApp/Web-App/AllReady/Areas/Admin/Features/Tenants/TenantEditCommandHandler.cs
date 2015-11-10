using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using AllReady.Areas.Admin.Models;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantEditCommandHandler : IRequestHandler<TenantEditCommand, int>
    {
        private AllReadyContext _context;

        public TenantEditCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public int Handle(TenantEditCommand message)
        {
            var tenant = _context
                    .Tenants
                    .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                    .Include(tc => tc.TenantContacts)
                    .SingleOrDefault(t => t.Id == message.Tenant.Id);
            if (tenant == null)
            {
                tenant = new Tenant();
            }
            tenant.Name = message.Tenant.Name;
            tenant.LogoUrl = message.Tenant.LogoUrl;
            tenant.WebUrl = message.Tenant.WebUrl;

            var contactInfo = string.Concat(message.Tenant.PrimaryContactEmail?.Trim() + message.Tenant.PrimaryContactFirstName?.Trim(), message.Tenant.PrimaryContactLastName?.Trim(), message.Tenant.PrimaryContactPhoneNumber?.Trim());
            var primaryTenantContact = tenant.TenantContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactType.Primary);
            var contactId = primaryTenantContact?.ContactId;
            Contact primaryContact;

            if (contactId == null)
            {
                primaryContact = new Contact();
            }
            else
            {
                primaryContact = _context.Contacts.Single(c => c.Id == contactId);
            }
            if (string.IsNullOrWhiteSpace(contactInfo) && primaryTenantContact != null)
            {
                //Delete
                _context.TenantContacts.Remove(primaryTenantContact);
                _context.Remove(primaryTenantContact);
                _context.Remove(primaryTenantContact.Contact);

            }
            else
            {
                primaryContact.Email = message.Tenant.PrimaryContactEmail;
                primaryContact.FirstName = message.Tenant.PrimaryContactFirstName;
                primaryContact.LastName = message.Tenant.PrimaryContactLastName;
                primaryContact.PhoneNumber = message.Tenant.PrimaryContactPhoneNumber;
                _context.Update(primaryContact);

                if (primaryTenantContact == null)
                {
                    primaryTenantContact = new TenantContact
                    {
                        Contact = primaryContact,
                        Tenant = tenant,
                        ContactType = (int)ContactType.Primary
                    };
                    tenant.TenantContacts.Add(primaryTenantContact);
                    _context.Update(primaryTenantContact);
                }

            }
            if (message.Tenant.Location != null)
            {
                var locationInfo = string.Concat(message.Tenant.Location?.Address1?.Trim(), message.Tenant.Location?.Address2?.Trim(), message.Tenant.Location?.City?.Trim(), message.Tenant.Location?.State?.Trim(), message.Tenant.Location?.PostalCode?.Trim(), message.Tenant.Location?.Name?.Trim(), message.Tenant.Location?.PhoneNumber?.Trim());
                if (string.IsNullOrWhiteSpace(locationInfo))
                {
                    tenant.Location = null;
                }
                else
                {
                    if (tenant.Location == null || message.Tenant.Location.Id.GetValueOrDefault() != 0)
                    {
                        tenant.Location = new Location();
                    }
                    tenant.Location = ToModel(tenant.Location, message.Tenant.Location);
                }
                _context.Update(tenant.Location);
            }
            _context.Update(tenant);
            _context.SaveChanges();
            return tenant.Id;

        }

        private Location ToModel(Location location, LocationEditModel locationEditModel)
        {
            location.Address1 = locationEditModel.Address1;
            location.Address2 = locationEditModel.Address2;
            location.City = locationEditModel.City;
            location.Country = locationEditModel.Country;
            location.Name = locationEditModel.Name;
            location.PhoneNumber = locationEditModel.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(locationEditModel.PostalCode)){
                location.PostalCode = new PostalCodeGeo { PostalCode = locationEditModel.PostalCode, City = locationEditModel.City, State = locationEditModel.State };
            }
            location.State = locationEditModel.State;
            return location;
        }
    }
}
