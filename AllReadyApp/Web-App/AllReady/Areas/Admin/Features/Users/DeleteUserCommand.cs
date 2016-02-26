using MediatR;

namespace AllReady.Areas.Admin.Features.Users
{
    public class DeleteUserCommand : IAsyncRequest
    {
        public string UserId { get; set; }
    }
}
