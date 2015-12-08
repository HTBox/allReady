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

            var tenant = _context.Organizations.SingleOrDefault(t => t.Id == message.Id);
            if (tenant != null)
            {
                _context.Organizations.Remove(tenant);
                _context.SaveChanges();
            }
        }
    }
}
