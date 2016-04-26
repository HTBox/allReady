using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class UnregisterEventHandler : AsyncRequestHandler<UnregisterEvent>
    {
        private readonly IAllReadyDataAccess dataAccess;
        private readonly IMediator mediator;

        public UnregisterEventHandler(IAllReadyDataAccess dataAccess, IMediator mediator)
        {
            this.dataAccess = dataAccess;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(UnregisterEvent message)
        {
            await dataAccess.DeleteEventAndTaskSignupsAsync(message.EventSignupId).ConfigureAwait(false);
            
            //Notify admins & volunteer
            await mediator.PublishAsync(new UserUnenrolls { EventId = message.EventSignupId, UserId = message.UserId }).ConfigureAwait(false);
        }
    }
}