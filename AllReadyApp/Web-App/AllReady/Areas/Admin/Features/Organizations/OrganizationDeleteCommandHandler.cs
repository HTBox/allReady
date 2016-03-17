using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDeleteCommandHandler : RequestHandler<OrganizationDeleteCommand>
    {
        private AllReadyContext _context;
        public OrganizationDeleteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override void HandleCore(OrganizationDeleteCommand message) {

            var organization = _context.Organizations.SingleOrDefault(t => t.Id == message.Id);
            if (organization != null)
            {
                _context.Organizations.Remove(organization);
                _context.SaveChanges();
            }
        }
    }
}
