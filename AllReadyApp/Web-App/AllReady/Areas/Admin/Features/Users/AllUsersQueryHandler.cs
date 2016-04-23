using System.Collections.Generic;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Users
{
    public class AllUsersQueryHandler : IRequestHandler<AllUsersQuery, IEnumerable<ApplicationUser>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public AllUsersQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public IEnumerable<ApplicationUser> Handle(AllUsersQuery message)
        {
            return dataAccess.Users;
        }
    }
}