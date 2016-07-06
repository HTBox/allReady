using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationContactQueryHandlerAsync : IAsyncRequestHandler<OrganizationContactQuery , ContactInformationModel>
    {
        private readonly AllReadyContext _context;
        public OrganizationContactQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<ContactInformationModel> Handle(OrganizationContactQuery  message)
        {
            var contactInfo = new ContactInformationModel();

            var organization = await _context.Organizations
                .AsNoTracking()
                .Include(l => l.Location).ThenInclude(pc => pc.PostalCode)
                .Include(oc => oc.OrganizationContacts).ThenInclude(c => c.Contact)
                .SingleOrDefaultAsync(o => o.Id == message.OrganizationId)
                .ConfigureAwait(false);

            if (organization == null)
            {
                return contactInfo;
            }

            if (organization.Location != null)
            {
                contactInfo.Location = organization.Location.ToModel();
            }
            
            var contact = organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact;
            if (contact != null)
            {
                contactInfo.Email = contact.Email;
                contactInfo.FirstName = contact.FirstName;
                contactInfo.LastName = contact.LastName;
                contactInfo.PhoneNumber = contact.PhoneNumber;
            }

            return contactInfo;
        }
    }
}
