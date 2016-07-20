using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQueryHandlerAsync : IAsyncRequestHandler<OrganizationEditQueryAsync, OrganizationEditModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationEditQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationEditModel> Handle(OrganizationEditQueryAsync message)
        {
            var org = await _context.Organizations
                .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location)
                .Include(u => u.Users).Include(tc => tc.OrganizationContacts)
                .ThenInclude(c => c.Contact)
                .SingleOrDefaultAsync(ten => ten.Id == message.Id)
                .ConfigureAwait(false);

            if (org == null)
            {
                return null;
            }

            var organization = new OrganizationEditModel
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
                organization = (OrganizationEditModel)org.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(organization);
            }
            
            return organization;
        }
    }
}
