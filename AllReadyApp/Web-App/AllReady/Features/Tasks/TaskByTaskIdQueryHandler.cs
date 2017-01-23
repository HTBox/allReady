using AllReady.Models;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Features.Tasks
{
    public class TaskByTaskIdQueryHandler : IAsyncRequestHandler<TaskByTaskIdQuery, VolunteerTask>
    {
        private readonly AllReadyContext dataContext;

        public TaskByTaskIdQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<VolunteerTask> Handle(TaskByTaskIdQuery message)
        {
            return await dataContext.Tasks
                .Include(x => x.Organization)
                .Include(x => x.Event)
                .Include(x => x.Event.Campaign)
                .Include(x => x.AssignedVolunteers).ThenInclude(v => v.User)
                .Include(x => x.RequiredSkills)
                .Where(t => t.Id == message.TaskId)
                .SingleOrDefaultAsync();
        }
    }
}
