using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    public class ManagingOrganizationIdByEventIdQueryHandler : IAsyncRequestHandler<ManagingOrganizationIdByEventIdQuery, int>
    {
        private AllReadyContext _context;

        public ManagingOrganizationIdByEventIdQueryHandler(AllReadyContext context)
        {
            _context= context;
        }

        public async Task<int> Handle(ManagingOrganizationIdByEventIdQuery message)
        {
            return await _context.Events.Where(a => a.Id == message.EventId)
                .Select(a => a.Campaign.ManagingOrganizationId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}
