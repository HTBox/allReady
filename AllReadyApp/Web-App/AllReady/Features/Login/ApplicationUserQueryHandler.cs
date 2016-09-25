using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Login
{
    public class ApplicationUserQueryHandler : IAsyncRequestHandler<ApplicationUserQuery, ApplicationUser>
    {
        private AllReadyContext _context;

        public ApplicationUserQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public async Task<ApplicationUser> Handle(ApplicationUserQuery message)
        {
            var normalizedUserName = message.UserName.ToUpperInvariant();
            var user = await _context.Users
                .AsNoTracking()
                .Include(a => a.Claims)
                .Include(a => a.Roles)
                .SingleOrDefaultAsync(a => a.NormalizedUserName == normalizedUserName)
                .ConfigureAwait(false);
             return user;
        }
    }
}