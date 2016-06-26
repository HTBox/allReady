using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Manage
{
    public class UpdateUserHandler : AsyncRequestHandler<UpdateUser>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public UpdateUserHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task HandleCore(UpdateUser message)
        {
            await dataAccess.UpdateUser(message.User);
        }
    }
}
