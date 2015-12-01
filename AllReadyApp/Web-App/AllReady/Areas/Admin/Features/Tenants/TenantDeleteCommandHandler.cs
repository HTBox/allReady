using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantDeleteCommandHandler : RequestHandler<TenantDeleteCommand>
    {
        private AllReadyContext _context;
        public TenantDeleteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override void HandleCore(TenantDeleteCommand message) {

            var tenant = _context.Tenants.SingleOrDefault(t => t.Id == message.Id);
            if (tenant != null)
            {
                _context.Tenants.Remove(tenant);
                _context.SaveChanges();
            }
        }
    }
}
