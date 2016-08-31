using System.Threading.Tasks;
using System.Linq;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class UnregisterEventHandler : AsyncRequestHandler<UnregisterEvent> {
        private readonly AllReadyContext dataContext;
        private readonly IMediator mediator;

        public UnregisterEventHandler(AllReadyContext dataContext, IMediator mediator) {
            this.dataContext = dataContext;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(UnregisterEvent message) {
            await this.DeleteEventAndTaskSignupsAsync(message.EventSignupId).ConfigureAwait(false);

            //Notify admins & volunteer
            await mediator.PublishAsync(new UserUnenrolls {EventId = message.EventSignupId, UserId = message.UserId}).ConfigureAwait(false);
        }

        // public to be testable, not meant for public consumption
        public Task DeleteEventAndTaskSignupsAsync(int eventSignupId) {
            var eventSignup = this.dataContext.EventSignup.SingleOrDefault(c => c.Id == eventSignupId);

            if (eventSignup == null) {
                return Task.FromResult(0);
            }

            this.dataContext.EventSignup.Remove(eventSignup);

            this.dataContext.TaskSignups.RemoveRange(this.dataContext.TaskSignups
                .Where(e => e.Task.Event.Id == eventSignup.Event.Id)
                .Where(e => e.User.Id == eventSignup.User.Id));

            return this.dataContext.SaveChangesAsync();
        }
    }
}
