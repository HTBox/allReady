using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDeleteCommandHandlerAsync : AsyncRequestHandler<OrganizationDeleteCommand>
    {
        private AllReadyContext _context;
        public OrganizationDeleteCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(OrganizationDeleteCommand message)
        {
            var organization = await _context.Organizations.SingleOrDefaultAsync(t => t.Id == message.Id).ConfigureAwait(false);
            if (organization != null)
            {
                _context.Organizations.Remove(organization);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
