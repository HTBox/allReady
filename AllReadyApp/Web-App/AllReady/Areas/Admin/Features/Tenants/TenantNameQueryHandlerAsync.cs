using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantNameQueryHandlerAsync : IAsyncRequestHandler<TenantNameQueryAsync, string>
    {
        private AllReadyContext _context;
        public TenantNameQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(TenantNameQueryAsync message)
        {
            return await _context.Organizations
                .Where(t => t.Id == message.Id)
                .Select(t => t.Name)
                .SingleOrDefaultAsync();
        }
    }
}