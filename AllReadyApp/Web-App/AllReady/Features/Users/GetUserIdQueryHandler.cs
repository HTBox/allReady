using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

namespace AllReady.Features.Users
{
    public class GetUserIdQueryHandler : IAsyncRequestHandler<GetUserIdQuery, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserIdQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(GetUserIdQuery message)
        {
            var user = await _userManager.GetUserAsync(message.User);

            return user?.Id;
        }
    }
}
