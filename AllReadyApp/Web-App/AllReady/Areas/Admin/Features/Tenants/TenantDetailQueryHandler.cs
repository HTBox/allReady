using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantDetailQueryHandler : IRequestHandler<TenantDetailQuery, TenantDetailModel>
    {
        private AllReadyContext _context;
        public TenantDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public TenantDetailModel Handle(TenantDetailQuery message)
        {
            var t = _context.Tenants
                 .AsNoTracking()
                .Include(c => c.Campaigns)
                .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                .Include(u => u.Users)
                .Include(c => c.TenantContacts).ThenInclude(tc => tc.Contact)
                .Where(ten => ten.Id == message.Id)
                .SingleOrDefault();
            if (t == null)
            {
                return null;
            }
            var tenant = new TenantDetailModel
            {
                Id = t.Id,
                Name = t.Name,
                Location = t.Location.ToModel(),
                LogoUrl = t.LogoUrl,
                WebUrl = t.WebUrl,
                Campaigns = t.Campaigns,
                Users = t.Users,
            };
            if (t.TenantContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
            {
                tenant = (TenantDetailModel)t.TenantContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(tenant);
            }
            return tenant;
        }

     
    }
}
