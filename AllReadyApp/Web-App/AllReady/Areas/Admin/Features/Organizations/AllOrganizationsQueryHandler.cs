using System.Collections.Generic;
using AllReady.Models;
using MediatR;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class AllOrganizationsQueryHandler : IAsyncRequestHandler<AllOrganizationsQuery, IEnumerable<Organization>>
    {
        private readonly AllReadyContext _context;

        public AllOrganizationsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organization>> Handle(AllOrganizationsQuery message)
        {
            return await _context.Organizations.ToListAsync();
        }
    }
}