using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantEditQueryHandler : IRequestHandler<TenantEditQuery, TenantEditModel>
    {
        private AllReadyContext _context;
        public TenantEditQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public TenantEditModel Handle(TenantEditQuery message)
        {
            var t = _context.Tenants
                 .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location).ThenInclude(p=>p.PostalCode)
                .Include(u => u.Users)
                .Include(tc=> tc.TenantContacts)
                .Where(ten => ten.Id == message.Id)
                .SingleOrDefault();
            if (t == null)
            {
                return null;
            }
            var tenant = new TenantEditModel
            {
                Id = t.Id,
                Name = t.Name,
                Location = ToModel(t.Location),
                LogoUrl = t.LogoUrl,
                WebUrl = t.WebUrl,
            };
            var contactId = t.TenantContacts?.SingleOrDefault(tc => tc.ContactType ==(int) ContactType.Primary)?.ContactId;
            if (contactId != null)
            {
                var contact = _context.Contacts.Single(c => c.Id == contactId);
                tenant.PrimaryContactEmail = contact.Email;
                tenant.PrimaryContactFirstName = contact.FirstName;
                tenant.PrimaryContactLastName = contact.LastName;
                tenant.PrimaryContactPhoneNumber = contact.PhoneNumber;
            }
            return tenant;
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
