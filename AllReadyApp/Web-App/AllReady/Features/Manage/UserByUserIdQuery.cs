using AllReady.Models;
using MediatR;

namespace AllReady.Features.Manage
{
    public class UserByUserIdQuery : IRequest<ApplicationUser>
    {
        public string UserId { get; set; }
    }
}
