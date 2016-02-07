using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Login
{
    public class RemoveUserProfileIncompleteClaimCommandHandler : AsyncRequestHandler<RemoveUserProfileIncompleteClaimCommand>
    {
        private AllReadyContext _context;

        public RemoveUserProfileIncompleteClaimCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected async override System.Threading.Tasks.Task HandleCore(RemoveUserProfileIncompleteClaimCommand message)
        {
            //Going directly to the database here to remove the claim instead of using the UserManager because the user isn't always logged in (eg. when confirming email address)
            var existingClaim = await _context.UserClaims.SingleOrDefaultAsync(u => u.UserId == message.UserId && u.ClaimType == Security.ClaimTypes.ProfileIncomplete);
            if (existingClaim != null)
            {
                _context.Remove(existingClaim);
                await _context.SaveChangesAsync();
            }
        }
    }
}