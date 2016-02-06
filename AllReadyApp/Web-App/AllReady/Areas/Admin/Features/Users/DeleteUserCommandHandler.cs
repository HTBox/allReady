using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Users
{
    public class DeleteUserCommandHandler : RequestHandler<DeleteUserCommand>
    {
        private readonly AllReadyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(AllReadyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected override void HandleCore(DeleteUserCommand message)
        {
            var user = _userManager.FindByIdAsync(message.UserId).Result;
            _userManager.DeleteAsync(user);
        }
    }
}
