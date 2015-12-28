using AllReady.Models;
using MediatR;

namespace AllReady.Features.Login
{
    public class ApplicationUserQuery : IAsyncRequest<ApplicationUser>
    {
        public string UserName { get; set; }
    }
}
