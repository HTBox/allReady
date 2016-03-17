using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameQueryHandlerAsync : IAsyncRequestHandler<OrganizationNameQueryAsync, string>
    {
        private AllReadyContext _context;
        public OrganizationNameQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(OrganizationNameQueryAsync message)
        {
            return await _context.Organizations
                .Where(t => t.Id == message.Id)
                .Select(t => t.Name)
                .SingleOrDefaultAsync();
        }
    }
}