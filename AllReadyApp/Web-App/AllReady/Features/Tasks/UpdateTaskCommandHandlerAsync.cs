using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class UpdateTaskCommandHandlerAsync : AsyncRequestHandler<UpdateTaskCommandAsync>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public UpdateTaskCommandHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(UpdateTaskCommandAsync message)
        {
            await dataAccess.UpdateTaskAsync(message.AllReadyTask);
        }
    }
}
