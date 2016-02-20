using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Users
{
    public class DeleteUserCommandHandler : AsyncRequestHandler<DeleteUserCommand>
    {
        private readonly AllReadyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(AllReadyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected override async Task HandleCore(DeleteUserCommand message)
        {
            var user = await _userManager.FindByIdAsync(message.UserId);
            await _userManager.DeleteAsync(user);
        }
    }
}
