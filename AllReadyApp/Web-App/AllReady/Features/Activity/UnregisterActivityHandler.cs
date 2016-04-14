using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class UnregisterActivityHandler : AsyncRequestHandler<UnregisterActivity>
    {
        private readonly IAllReadyDataAccess dataAccess;
        private readonly IMediator mediator;

        public UnregisterActivityHandler(IAllReadyDataAccess dataAccess, IMediator mediator)
        {
            this.dataAccess = dataAccess;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(UnregisterActivity message)
        {
            await dataAccess.DeleteActivityAndTaskSignupsAsync(message.ActivitySignupId).ConfigureAwait(false);
            
            //Notify admins & volunteer
            await mediator.PublishAsync(new UserUnenrolls { ActivityId = message.ActivitySignupId, UserId = message.UserId }).ConfigureAwait(false);
        }
    }
}