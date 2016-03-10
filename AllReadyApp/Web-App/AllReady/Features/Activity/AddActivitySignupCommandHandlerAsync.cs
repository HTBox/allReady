using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class AddActivitySignupCommandHandlerAsync : AsyncRequestHandler<AddActivitySignupCommandAsync>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public AddActivitySignupCommandHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(AddActivitySignupCommandAsync message)
        {
            await dataAccess.AddActivitySignupAsync(message.ActivitySignup);
        }
    }
}
