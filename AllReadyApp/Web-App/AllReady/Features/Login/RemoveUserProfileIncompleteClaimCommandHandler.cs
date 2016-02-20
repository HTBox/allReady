using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AllReady.Features.Login
{
    public class RemoveUserProfileIncompleteClaimCommandHandler : AsyncRequestHandler<RemoveUserProfileIncompleteClaimCommand>
    {
        private AllReadyContext _context;
        private SignInManager<ApplicationUser> _signInManager;

        public RemoveUserProfileIncompleteClaimCommandHandler(AllReadyContext context,
                                                              SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _context = context;
        }

        protected async override Task HandleCore(RemoveUserProfileIncompleteClaimCommand message)
        {
            //Going directly to the database here to remove the claim instead of using the UserManager because the user isn't always logged in (eg. when confirming email address)
            var existingClaim = await _context.UserClaims.SingleOrDefaultAsync(u => u.UserId == message.UserId && u.ClaimType == Security.ClaimTypes.ProfileIncompleted);
            if (existingClaim != null)
            {
                _context.Remove(existingClaim);
                await _context.SaveChangesAsync();
            }
        }
    }
}