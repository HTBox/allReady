using AllReady.Models;
using MediatR;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameUniqueQueryHandlerAsync : IAsyncRequestHandler<OrganizationNameUniqueQueryAsync, bool>
    {
        private AllReadyContext _context;

        public OrganizationNameUniqueQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(OrganizationNameUniqueQueryAsync message)
        {
            var existingOrgCount = await _context.Organizations
                .CountAsync(o => o.Name == message.OrganizationName && o.Id != message.OrganizationId)
                .ConfigureAwait(false);

            return existingOrgCount <= 0;
        }
    }
}
