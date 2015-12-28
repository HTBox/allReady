using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQueryHandler : IRequestHandler<OrganizationEditQuery, OrganizationEditModel>
    {
        private AllReadyContext _context;
        public OrganizationEditQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public OrganizationEditModel Handle(OrganizationEditQuery message)
        {
            var t = _context.Organizations
                 .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                .Include(u => u.Users)
                .Include(tc => tc.OrganizationContacts).ThenInclude(c => c.Contact)
                .Where(ten => ten.Id == message.Id)
                .SingleOrDefault();
            if (t == null)
            {
                return null;
            }
            var organization = new OrganizationEditModel
            {
                Id = t.Id,
                Name = t.Name,
                Location = t.Location.ToEditModel(),
                LogoUrl = t.LogoUrl,
                WebUrl = t.WebUrl,
            };

            if (t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
            {
                organization = (OrganizationEditModel)t.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(organization);
            }
            return organization;
        }



    }
}
