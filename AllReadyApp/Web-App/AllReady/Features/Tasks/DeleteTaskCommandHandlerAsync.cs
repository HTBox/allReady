using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class DeleteTaskCommandHandlerAsync : AsyncRequestHandler<DeleteTaskCommandAsync>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public DeleteTaskCommandHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(DeleteTaskCommandAsync message)
        {
            await dataAccess.DeleteTaskAsync(message.TaskId);
        }
    }
}
