using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class OrganizationIdByVolunteerTaskIdQueryHandler : IAsyncRequestHandler<OrganizationIdByVolunteerTaskIdQuery, int>
    {
        private readonly AllReadyContext _context;

        public OrganizationIdByVolunteerTaskIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(OrganizationIdByVolunteerTaskIdQuery message)
        {
            var volunteerTask = await _context.VolunteerTasks
                .AsNoTracking()
                .Include(t => t.Organization)
                .SingleAsync(t => t.Id == message.VolunteerTaskId);

            return volunteerTask.Organization.Id;
        }
    }
}