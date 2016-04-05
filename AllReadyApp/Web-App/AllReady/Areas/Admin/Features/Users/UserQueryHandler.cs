using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Users
{
    public class UserQueryHandler : IRequestHandler<UserQuery, EditUserModel>
    {
        private readonly AllReadyContext _context;

        public UserQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public EditUserModel Handle(UserQuery message)
        {
            var user = _context.Users
                .Where(u => u.Id == message.UserId)
                .Include(u => u.AssociatedSkills).ThenInclude(us => us.Skill)
                .Include(u => u.Claims)
                .SingleOrDefault();

            var organizationId = user.GetOrganizationId();

            var viewModel = new EditUserModel
            {
                UserId = message.UserId,
                UserName = user.UserName,
                AssociatedSkills = user.AssociatedSkills,
                IsOrganizationAdmin = user.IsUserType(UserType.OrgAdmin),
                IsSiteAdmin = user.IsUserType(UserType.SiteAdmin),
                Organization = organizationId != null ? _context.Organizations.First(t=>t.Id == organizationId.Value) : null
            };

            return viewModel;
        }
    }
}
