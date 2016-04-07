using AllReady.Models;
using MediatR;

namespace AllReady.Features.Manage
{
    public class UserByUserIdQueryHandler : IRequestHandler<UserByUserIdQuery, ApplicationUser>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public UserByUserIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public ApplicationUser Handle(UserByUserIdQuery message)
        {
            return dataAccess.GetUser(message.UserId);
        }
    }
}
