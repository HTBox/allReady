using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Tasks
{
    public class DeleteVolunteerTaskCommandHandler : AsyncRequestHandler<DeleteVolunteerTaskCommand>
    {
        private readonly AllReadyContext dataContext;

        public DeleteVolunteerTaskCommandHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(DeleteVolunteerTaskCommand message)
        {
            var toDelete = dataContext.VolunteerTasks
                .Include(t => t.Attachments)
                .SingleOrDefault(t => t.Id == message.VolunteerTaskId);

            if (toDelete != null)
            {
                dataContext.Attachments.RemoveRange(toDelete.Attachments);
                dataContext.VolunteerTasks.Remove(toDelete);
                await dataContext.SaveChangesAsync();
            }
        }
    }
}
