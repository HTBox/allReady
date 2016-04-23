using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class UpdateEventHandler : AsyncRequestHandler<UpdateEvent>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public UpdateEventHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(UpdateEvent message)
        {
            await dataAccess.UpdateEvent(message.Event).ConfigureAwait(false);
        }
    }
}