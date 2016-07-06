using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQueryHandlerAsync : IAsyncRequestHandler<OrganizationDetailQuery, OrganizationDetailModel >
    {
        private AllReadyContext _context;

        public OrganizationDetailQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationDetailModel> Handle(OrganizationDetailQuery message)
        {
            var t = await _context.Organizations
                .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location)
                .Include(u => u.Users)
                .Include(c => c.OrganizationContacts).ThenInclude(tc => tc.Contact)
                .Where(ten => ten.Id == message.Id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (t == null)
            {
                return null;
            }

            var organization = new OrganizationDetailModel
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
                organization = (OrganizationDetailModel) t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int) ContactTypes.Primary)?.Contact.ToEditModel(organization);
            }

            return organization;
        }
    }
}