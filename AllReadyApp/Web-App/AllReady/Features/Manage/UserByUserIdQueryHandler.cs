using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Manage
{
    public class UserByUserIdQueryHandler : IAsyncRequestHandler<UserByUserIdQuery, ApplicationUser>
    {
        private readonly AllReadyContext dataContext;

        public UserByUserIdQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<ApplicationUser> Handle(UserByUserIdQuery message)
        {
            return await dataContext.Users
                .Where(u => u.Id == message.UserId)
                .Include(u => u.AssociatedSkills).ThenInclude((UserSkill us) => us.Skill)
                .Include(u => u.Claims)
                .SingleOrDefaultAsync();
        }
    }
}
