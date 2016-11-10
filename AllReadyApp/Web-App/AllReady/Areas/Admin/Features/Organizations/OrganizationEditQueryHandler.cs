using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Extensions;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQueryHandler : IAsyncRequestHandler<OrganizationEditQuery, OrganizationEditViewModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationEditQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationEditViewModel> Handle(OrganizationEditQuery message)
        {
            var org = await _context.Organizations
                .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location)
                .Include(u => u.Users).Include(tc => tc.OrganizationContacts)
                .ThenInclude(c => c.Contact)
                .SingleOrDefaultAsync(ten => ten.Id == message.Id);

            if (org == null)
            {
                return null;
            }

            var organization = new OrganizationEditViewModel
            {
                Id = org.Id,
                Name = org.Name,
                Location = org.Location.ToEditModel(),
                LogoUrl = org.LogoUrl,
                WebUrl = org.WebUrl,
                Description = org.DescriptionHtml,
                Summary =  org.Summary,
                PrivacyPolicy = org.PrivacyPolicy,
                PrivacyPolicyUrl = org.PrivacyPolicyUrl
            };

            if (org.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
            {
                organization = (OrganizationEditViewModel)org.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(organization);
            }
            
            return organization;
        }
    }
}
