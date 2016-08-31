using System.Linq;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Manage
{
    public class UserByUserIdQueryHandler : IRequestHandler<UserByUserIdQuery, ApplicationUser>
    {
        private readonly AllReadyContext dataContext;

        public UserByUserIdQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public ApplicationUser Handle(UserByUserIdQuery message)
        {
            return dataContext.Users
                .Where(u => u.Id == message.UserId)
                .Include(u => u.AssociatedSkills).ThenInclude((UserSkill us) => us.Skill)
                .Include(u => u.Claims)
                .SingleOrDefault();
        }
    }
}
