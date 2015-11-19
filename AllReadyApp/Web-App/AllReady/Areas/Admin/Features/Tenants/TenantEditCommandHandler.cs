using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using AllReady.Areas.Admin.Models;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantEditCommandHandler : IRequestHandler<TenantEditCommand, int>
    {
        private AllReadyContext _context;

        public TenantEditCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public int Handle(TenantEditCommand message)
        {
            var tenant = _context
                    .Tenants
                    .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                    .Include(tc => tc.TenantContacts)
                    .SingleOrDefault(t => t.Id == message.Tenant.Id);
            if (tenant == null)
            {
                tenant = new Tenant();
            }
            tenant.Name = message.Tenant.Name;
            tenant.LogoUrl = message.Tenant.LogoUrl;
            tenant.WebUrl = message.Tenant.WebUrl;

            tenant = tenant.UpdateTenantContact(message.Tenant, _context);
            tenant.Location = tenant.Location.UpdateModel(message.Tenant.Location);
            if (tenant.Location != null)
            {
                _context.Update(tenant.Location);
            }
            _context.Update(tenant);
            _context.SaveChanges();
            return tenant.Id;

        }

       
    }
}
