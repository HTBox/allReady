using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddVolunteerTaskCommandHandler : AsyncRequestHandler<AddVolunteerTaskCommand>
    {
        private readonly AllReadyContext dataContext;

        public AddVolunteerTaskCommandHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(AddVolunteerTaskCommand message)
        {
            dataContext.Add(message.VolunteerTask);
            await dataContext.SaveChangesAsync();
        }
    }
}
