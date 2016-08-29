using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddTaskCommandHandlerAsync : AsyncRequestHandler<AddTaskCommandAsync>
    {
        private readonly AllReadyContext dataContext;

        public AddTaskCommandHandlerAsync(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(AddTaskCommandAsync message)
        {
            this.dataContext.Add(message.AllReadyTask);
            await this.dataContext.SaveChangesAsync();
        }
    }
}
