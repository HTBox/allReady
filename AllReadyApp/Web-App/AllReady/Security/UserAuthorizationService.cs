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
                if (_user.Email == claimsPrincipal.Identity.Name)
                {
                    // This is probably a "re-executed" request that has already been
                    // through the pipeline once.
                    return;
                }

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
        public async Task<bool> IsCampaignManager()
        {
            var campaignsManaged = await GetManagedCampaignIds();

            return campaignsManaged.Any();
        }

        /// <inheritdoc />
        public async Task<bool> IsEventManager()
        {
            var eventsManaged = await GetManagedEventIds();

            return eventsManaged.Any();
        }

        /// <inheritdoc />
        public async Task<bool> IsTeamLead()
        {
            var teamsLed = await GetLedItineraryIds();

            return teamsLed.Any();
        }        

        /// <inheritdoc />
        public bool IsOrganizationAdmin(int organizationId)
        {
            return _claimsPrincipal.IsUserType(UserType.OrgAdmin) && GetOrganizationId.HasValue && GetOrganizationId.Value == organizationId;
        }

        private List<int> _ledItineraryIds;

        /// <inheritdoc />
        public async Task<List<int>> GetLedItineraryIds()
        {
            if(!HasAssociatedUser)
            {
                return new List<int>();
            }

            if (_ledItineraryIds != null)
            {
                return _ledItineraryIds;
            }

            _ledItineraryIds = await _context.VolunteerTaskSignups.AsNoTracking()
                    .Include(x => x.User)
                    .Where(x => x.User.Id == _user.Id && x.IsTeamLead && x.ItineraryId.HasValue)
                    .Select(x => x.ItineraryId.Value)
                    .ToListAsync();

            return _ledItineraryIds;
        }

        public int? GetOrganizationId
        {
            get
            {
                var organizationIdClaim = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Organization);

                if (int.TryParse(organizationIdClaim?.Value, out int organizationId))
                {
                    return organizationId;
                }

                return null;
            }
        }
    }
}
