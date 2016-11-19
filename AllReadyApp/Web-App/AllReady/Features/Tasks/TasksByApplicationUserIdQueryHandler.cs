using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Tasks
{
    public class TasksByApplicationUserIdQueryHandler : IAsyncRequestHandler<TasksByApplicationUserIdQuery, List<AllReadyTask>>
    {
        private readonly AllReadyContext _context;

        public TasksByApplicationUserIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<AllReadyTask>> Handle(TasksByApplicationUserIdQuery message)
        {
            return await _context.TaskSignups
                        .Include(x => x.User)
                        .Include(x => x.Task)
                        .Where(x => x.User.Id == message.ApplicationUserId)
                        .Select(x => x.Task)
                        .ToListAsync();
        }
    }
}
