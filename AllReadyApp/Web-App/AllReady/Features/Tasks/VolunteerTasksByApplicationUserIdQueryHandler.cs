using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Tasks
{
    public class VolunteerTasksByApplicationUserIdQueryHandler : IAsyncRequestHandler<VolunteerTasksByApplicationUserIdQuery, List<VolunteerTask>>
    {
        private readonly AllReadyContext _context;

        public VolunteerTasksByApplicationUserIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<VolunteerTask>> Handle(VolunteerTasksByApplicationUserIdQuery message)
        {
            return await _context.VolunteerTaskSignups
                .Include(x => x.User)
                .Include(x => x.VolunteerTask)
                .Where(x => x.User.Id == message.ApplicationUserId)
                .Select(x => x.VolunteerTask)
                .ToListAsync();
        }
    }
}
