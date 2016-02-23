using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationContactQueryHandler : IRequestHandler<OrganizationContactQuery , ContactInformationModel>
    {
        private AllReadyContext _context;
        public OrganizationContactQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public ContactInformationModel Handle(OrganizationContactQuery  message)
        {
            var contactInfo = new ContactInformationModel();
            var t = _context.Organizations
                .AsNoTracking()
               .Include(l => l.Location).ThenInclude(p => p.PostalCode)
               .Include(c => c.OrganizationContacts).ThenInclude(tc => tc.Contact)
               .Where(ten => ten.Id == message.Id)
               .SingleOrDefault();
            if (t == null)
            {
                return contactInfo;
            }

            if (t.Location != null) {
                contactInfo.Location = t.Location.ToModel();
            }            
            var contact = t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact;
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
