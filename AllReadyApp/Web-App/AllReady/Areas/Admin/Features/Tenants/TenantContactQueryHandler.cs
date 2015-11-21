using Microsoft.Data.Entity;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantContactQueryHandler : IRequestHandler<TenantContactQuery, ContactInformationModel>
    {
        private AllReadyContext _context;
        public TenantContactQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public ContactInformationModel Handle(TenantContactQuery message)
        {
            var contactInfo = new ContactInformationModel();
            var t = _context.Tenants
                .AsNoTracking()
               .Include(l => l.Location).ThenInclude(p => p.PostalCode)
               .Include(c => c.TenantContacts).ThenInclude(tc => tc.Contact)
               .Where(ten => ten.Id == message.Id)
               .SingleOrDefault();
            if (t == null)
            {
                return contactInfo;
            }

            if (t.Location != null) {
                contactInfo.Location = t.Location.ToModel();
            }            
            var contact = t.TenantContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact;
            if (contact != null)
            {
                //var contact = _context.Contacts.Single(c => c.Id == contactId);
                contactInfo.Email = contact.Email;
                contactInfo.FirstName = contact.FirstName;
                contactInfo.LastName = contact.LastName;
                contactInfo.PhoneNumber = contact.PhoneNumber;
            }

            return contactInfo;

        }
       
    }
}
