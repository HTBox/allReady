using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class OrganizationIdByTaskIdQueryHandler : IAsyncRequestHandler<OrganizationIdByTaskIdQuery, int>
    {
        private readonly AllReadyContext _context;

        public OrganizationIdByTaskIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(OrganizationIdByTaskIdQuery message)
        {
            var volunteerTask = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Organization)
                .SingleAsync(t => t.Id == message.VolunteerTaskId);

            return volunteerTask.Organization.Id;
        }
    }
}