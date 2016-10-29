using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Extensions;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQueryHandler : IAsyncRequestHandler<OrganizationDetailQuery, OrganizationDetailViewModel >
    {
        private AllReadyContext _context;

        public OrganizationDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationDetailViewModel> Handle(OrganizationDetailQuery message)
        {
            var t = await _context.Organizations
                .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location)
                .Include(u => u.Users)
                .Include(c => c.OrganizationContacts).ThenInclude(tc => tc.Contact)
                .Where(ten => ten.Id == message.Id)
                .SingleOrDefaultAsync();

            if (t == null)
            {
                return null;
            }

            var organization = new OrganizationDetailViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Location = t.Location.ToModel(),
                LogoUrl = t.LogoUrl,
                WebUrl = t.WebUrl,
                Campaigns = t.Campaigns,
                Users = t.Users,
            };

            if (t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int) ContactTypes.Primary)?.Contact != null)
            {
                organization = (OrganizationDetailViewModel) t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int) ContactTypes.Primary)?.Contact.ToEditModel(organization);
            }

            return organization;
        }
    }
}