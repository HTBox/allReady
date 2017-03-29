using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Security
{
    /// <summary>
    /// A service which provides information to enable authorization of the current user for application actions
    /// </summary>
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AllReadyContext _context;

        private ApplicationUser _user;
        private ClaimsPrincipal _claimsPrincipal;

        public UserAuthorizationService(UserManager<ApplicationUser> userManager, AllReadyContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <inheritdoc />
        public async Task AssociateUser(ClaimsPrincipal claimsPrincipal)
        {
            if (HasAssociatedUser)
            {
                throw new InvalidOperationException("AssociateUser cannot be called when a user has been previously associated");
            }

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                _claimsPrincipal = claimsPrincipal;

                var email = claimsPrincipal.Identity.Name;

                _user = await _userManager.FindByEmailAsync(email);
            }
        }

        /// <inheritdoc />
        public bool HasAssociatedUser => _user != null;

        /// <inheritdoc />
        public string AssociatedUserId => _user?.Id;

        private List<int> _managedEventIds;

        /// <inheritdoc />
        /// <remarks>This will be lazy loaded from the database upon on first access</remarks>
        public async Task<List<int>> GetManagedEventIds()
        {
            if (!HasAssociatedUser)
            {
                return new List<int>();
            }

            if (_managedEventIds != null)
            {
                return _managedEventIds;
            }

            _managedEventIds = await _context.EventManagers.AsNoTracking()
                    .Where(x => x.UserId == _user.Id)
                    .Select(x => x.EventId)
                    .ToListAsync();

            return _managedEventIds;
        }

        private List<int> _managedCampaignIds;

        /// <inheritdoc />
        /// <remarks>This will be lazy loaded from the database upon on first access</remarks>
        public async Task<List<int>> GetManagedCampaignIds()
        {
            if (!HasAssociatedUser)
            {
                return new List<int>();
            }

            if (_managedCampaignIds != null)
            {
                return _managedCampaignIds;
            }

            _managedCampaignIds = await _context.CampaignManagers.AsNoTracking()
                    .Where(x => x.UserId == _user.Id)
                    .Select(x => x.CampaignId)
                    .ToListAsync();

            return _managedCampaignIds;
        }

        /// <inheritdoc />
        public bool IsSiteAdmin
        {
            get
            {
                return _claimsPrincipal?.Claims != null &&
                    _claimsPrincipal.Claims.Any(
                        c =>
                            c.Type == ClaimTypes.UserType &&
                            c.Value == Enum.GetName(typeof(UserType), UserType.SiteAdmin));
            }
        }

        /// <inheritdoc />
        public bool IsOrganizationAdmin(int organizationId)
        {
            return _claimsPrincipal.IsUserType(UserType.OrgAdmin) && GetOrganizationId.HasValue && GetOrganizationId.Value == organizationId;
        }

        private int? GetOrganizationId
        {
            get
            {
                int? result = null;
                var organizationIdClaim = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Organization);

                if (organizationIdClaim == null)
                {
                    return null;
                }

                int organizationId;
                if (int.TryParse(organizationIdClaim.Value, out organizationId))
                { 
                    result = organizationId;
                }

                return result;
            }
        }
    }
}
