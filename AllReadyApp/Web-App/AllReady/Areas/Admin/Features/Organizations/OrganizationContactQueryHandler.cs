using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.OrganizationApi;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationContactQueryHandler : IAsyncRequestHandler<OrganizationContactQuery , ContactInformationViewModel>
    {
        private readonly AllReadyContext _context;
        public OrganizationContactQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<ContactInformationViewModel> Handle(OrganizationContactQuery  message)
        {
            var contactInfo = new ContactInformationViewModel();

            var organization = await _context.Organizations
                .AsNoTracking()
                .Include(l => l.Location)
                .Include(oc => oc.OrganizationContacts).ThenInclude(c => c.Contact)
                .SingleOrDefaultAsync(o => o.Id == message.OrganizationId);

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
