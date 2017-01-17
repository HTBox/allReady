using MediatR;
using System.Security.Claims;

namespace AllReady.Features.Users
{
    public class GetUserIdCommand : IAsyncRequest<string>
    {
        public ClaimsPrincipal User { get; set; }
    }
}
