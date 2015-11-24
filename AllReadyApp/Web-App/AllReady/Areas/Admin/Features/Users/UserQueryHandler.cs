using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Users
{
    public class UserQueryHandler : IRequestHandler<UserQuery, EditUserModel>
    {
        private readonly AllReadyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserQueryHandler(AllReadyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public EditUserModel Handle(UserQuery message)
        {
            var user = _context.Users
                .Where(u => u.Id == message.UserId)
                .Include(u => u.AssociatedSkills).ThenInclude((UserSkill us) => us.Skill)
                .Include(u => u.Claims)
                .SingleOrDefault();

            var tennantId = user.GetTenantId();

            var viewModel = new EditUserModel()
            {
                UserId = message.UserId,
                UserName = user.UserName,
                AssociatedSkills = user.AssociatedSkills,
                IsTenantAdmin = user.IsUserType(UserType.TenantAdmin),
                IsSiteAdmin = user.IsUserType(UserType.SiteAdmin),
                Tenant = tennantId != null ? _context.Tenants.First(t=>t.Id == tennantId.Value) : null
            };

            return viewModel;
        }
    }
}
