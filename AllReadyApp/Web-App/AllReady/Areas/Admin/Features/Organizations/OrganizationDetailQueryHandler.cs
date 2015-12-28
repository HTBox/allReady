using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQueryHandler : IRequestHandler<OrganizationDetailQuery, OrganizationDetailModel >
    {
        private AllReadyContext _context;
        public OrganizationDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public OrganizationDetailModel  Handle(OrganizationDetailQuery message)
        {
            var t = _context.Organizations
                 .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                .Include(u => u.Users)
                .Include(c => c.OrganizationContacts).ThenInclude(tc => tc.Contact)
                .Where(ten => ten.Id == message.Id)
                .SingleOrDefault();
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
            if (t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
            {
                organization = (OrganizationDetailModel )t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(organization);
            }
            return organization;
        }

     
    }
}
