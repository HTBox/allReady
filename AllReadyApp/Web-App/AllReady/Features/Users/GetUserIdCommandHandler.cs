using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

namespace AllReady.Features.Users
{
    public class GetUserIdCommandHandler : IAsyncRequestHandler<GetUserIdCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserIdCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(GetUserIdCommand message)
        {
            var user = await _userManager.GetUserAsync(message.User);

            return user?.Id;
        }
    }
}
