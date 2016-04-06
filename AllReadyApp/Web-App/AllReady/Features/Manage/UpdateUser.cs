using AllReady.Models;
using MediatR;

namespace AllReady.Features.Manage
{
    public class UpdateUser : IAsyncRequest
    {
        public ApplicationUser User { get; set; }
    }
}
