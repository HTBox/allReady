using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddTaskCommandHandlerAsync : AsyncRequestHandler<AddTaskCommandAsync>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public AddTaskCommandHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(AddTaskCommandAsync message)
        {
            await dataAccess.AddTaskAsync(message.AllReadyTask);
        }
    }
}
