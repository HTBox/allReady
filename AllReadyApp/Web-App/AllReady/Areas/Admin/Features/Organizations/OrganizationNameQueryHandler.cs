using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameQueryHandler : IAsyncRequestHandler<OrganizationNameQuery, string>
    {
        private AllReadyContext _context;
        public OrganizationNameQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(OrganizationNameQuery message)
        {
            return await _context.Organizations
                .Where(t => t.Id == message.Id)
                .Select(t => t.Name)
                .SingleOrDefaultAsync();
        }
    }
}