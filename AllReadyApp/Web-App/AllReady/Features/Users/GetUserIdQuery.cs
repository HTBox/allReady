using MediatR;
using System.Security.Claims;

namespace AllReady.Features.Users
{
    public class GetUserIdQuery : IAsyncRequest<string>
    {
        public ClaimsPrincipal User { get; set; }
    }
}
