using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class OrganizationIdByTaskIdQueryHandlerAsync : IAsyncRequestHandler<OrganizationIdByTaskIdQueryAsync, int>
    {
        private readonly AllReadyContext _context;

        public OrganizationIdByTaskIdQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(OrganizationIdByTaskIdQueryAsync message)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Organization)
                .SingleAsync(t => t.Id == message.TaskId)
                .ConfigureAwait(false);

            return task.Organization.Id;
        }
    }
}