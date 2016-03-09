using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class DeleteActivityAndTaskSignupsCommandHandlerAsync : AsyncRequestHandler<DeleteActivityAndTaskSignupsCommandAsync>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public DeleteActivityAndTaskSignupsCommandHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(DeleteActivityAndTaskSignupsCommandAsync message)
        {
            await dataAccess.DeleteActivityAndTaskSignupsAsync(message.ActivitySignupId);
        }
    }
}