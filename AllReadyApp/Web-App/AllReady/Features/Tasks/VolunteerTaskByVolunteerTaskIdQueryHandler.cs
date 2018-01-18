using AllReady.Models;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Features.Tasks
{
    public class VolunteerTaskByVolunteerTaskIdQueryHandler : IAsyncRequestHandler<VolunteerTaskByVolunteerTaskIdQuery, VolunteerTask>
    {
        private readonly AllReadyContext dataContext;

        public VolunteerTaskByVolunteerTaskIdQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<VolunteerTask> Handle(VolunteerTaskByVolunteerTaskIdQuery message)
        {
            return await dataContext.VolunteerTasks
                .Include(x => x.Organization)
                .Include(x => x.Event).ThenInclude(x => x.Campaign)
                .Include(x => x.AssignedVolunteers).ThenInclude(v => v.User)
                .Include(x => x.RequiredSkills)
                .Where(t => t.Id == message.VolunteerTaskId)
                .SingleOrDefaultAsync();
        }
    }
}
