using AllReady.Models;
using MediatR;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameUniqueQueryHandler : IAsyncRequestHandler<OrganizationNameUniqueQuery, bool>
    {
        private AllReadyContext _context;

        public OrganizationNameUniqueQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(OrganizationNameUniqueQuery message)
        {
            var existingOrgCount = await _context.Organizations
                .CountAsync(o => o.Name == message.OrganizationName && o.Id != message.OrganizationId);

            return existingOrgCount <= 0;
        }
    }
}
