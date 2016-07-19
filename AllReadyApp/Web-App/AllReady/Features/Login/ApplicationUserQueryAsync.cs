using AllReady.Models;
using MediatR;

namespace AllReady.Features.Login
{
    public class ApplicationUserQueryAsync : IAsyncRequest<ApplicationUser>
    {
        public string UserName { get; set; }
    }
}
