using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class DeleteOrganizationHandlerAsync : AsyncRequestHandler<DeleteOrganizationAsync>
    {
        private AllReadyContext _context;
        public DeleteOrganizationHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(DeleteOrganizationAsync message)
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
