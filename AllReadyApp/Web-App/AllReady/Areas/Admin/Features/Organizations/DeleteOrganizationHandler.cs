using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class DeleteOrganizationHandler : AsyncRequestHandler<DeleteOrganization>
    {
        private AllReadyContext _context;
        public DeleteOrganizationHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteOrganization message)
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
