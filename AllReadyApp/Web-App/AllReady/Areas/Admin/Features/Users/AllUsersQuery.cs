using System.Collections.Generic;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Users
{
    public class AllUsersQuery : IRequest<IEnumerable<ApplicationUser>>
    {
    }
}
