using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantEditQueryHandler : IRequestHandler<TenantEditQuery, TenantEditModel>
    {
        private AllReadyContext _context;
        public TenantEditQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public TenantEditModel Handle(TenantEditQuery message)
        {
            var t = _context.Tenants
                 .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                .Include(u => u.Users)
                .Include(tc => tc.TenantContacts).ThenInclude(c => c.Contact)
                .Where(ten => ten.Id == message.Id)
                .SingleOrDefault();
            if (t == null)
            {
                return null;
            }
            var tenant = new TenantEditModel
            {
                Id = t.Id,
                Name = t.Name,
                Location = t.Location.ToEditModel(),
                LogoUrl = t.LogoUrl,
                WebUrl = t.WebUrl,
            };

            if (t.TenantContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
            {
                tenant = (TenantEditModel)t.TenantContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(tenant);
            }
            return tenant;
        }



    }
}
