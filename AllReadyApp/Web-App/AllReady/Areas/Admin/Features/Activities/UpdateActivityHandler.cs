using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class UpdateActivityHandler : AsyncRequestHandler<UpdateActivity>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public UpdateActivityHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(UpdateActivity message)
        {
            await dataAccess.UpdateActivity(message.Activity).ConfigureAwait(false);
        }
    }
}