using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class AddEventSignupCommandHandlerAsync : AsyncRequestHandler<AddEventSignupCommandAsync>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public AddEventSignupCommandHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(AddEventSignupCommandAsync message)
        {
            await dataAccess.AddEventSignupAsync(message.EventSignup);
        }
    }
}
