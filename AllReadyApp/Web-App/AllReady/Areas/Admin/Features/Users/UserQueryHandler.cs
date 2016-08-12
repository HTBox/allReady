using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Site;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Users
{
    public class UserQueryHandler : IAsyncRequestHandler<UserQuery, EditUserModel>
    {
        private readonly AllReadyContext _context;

        public UserQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EditUserModel> Handle(UserQuery message)
        {
            var user = await _context.Users
                .Where(u => u.Id == message.UserId)
                .Include(u => u.AssociatedSkills).ThenInclude(us => us.Skill)
                .Include(u => u.Claims)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            var organizationId = user.GetOrganizationId();

            var viewModel = new EditUserModel
            {
                UserId = message.UserId,
                UserName = user.UserName,
                AssociatedSkills = user.AssociatedSkills,
                IsOrganizationAdmin = user.IsUserType(UserType.OrgAdmin),
                IsSiteAdmin = user.IsUserType(UserType.SiteAdmin),
                Organization = organizationId != null ? await _context.Organizations.FirstAsync(t=>t.Id == organizationId.Value).ConfigureAwait(false) : null
            };

            return viewModel;
        }
    }
}
