using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

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
            var toDelete = dataContext.Tasks.SingleOrDefault(t => t.Id == message.TaskId);

            if (toDelete != null)
            {
                dataContext.Tasks.Remove(toDelete);
                await dataContext.SaveChangesAsync();
            }
        }
    }
}
