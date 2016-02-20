using AllReady.Models;
using MediatR;

namespace AllReady.Features.Login
{
    public class RemoveUserProfileIncompleteClaimCommand : IAsyncRequest
    {
        public string UserId { get; set; }
    }
}
