using AllReady.Models;
using MediatR;

namespace AllReady.Features.Manage
{
    public class UserByUserIdQuery : IAsyncRequest<ApplicationUser>
    {
        public string UserId { get; set; }
    }
}
