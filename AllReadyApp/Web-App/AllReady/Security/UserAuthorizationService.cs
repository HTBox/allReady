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

        public bool HasAssociatedUser => _user != null;

        public string AssociatedUserId => _user.Id;
        
        private List<int> _managedEventIds;
        
        private async Task<List<int>> GetManagedEventIds()
        {
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

        private async Task<List<int>> GetManagedCampaignIds()
        {
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

        public async Task<bool> CanManageEvent(IAuthorizableEvent authorizableEvent)
        {
            if (!HasAssociatedUser)
            {
                return false;
            }

            if (_user.IsUserType(UserType.SiteAdmin) || _claimsPrincipal.IsOrganizationAdmin(authorizableEvent.OrganizationId))
            {
                return true;
            }

            var managedEventIds = await GetManagedEventIds();

            if (managedEventIds.Contains(authorizableEvent.EventId))
            {
                return true;
            }

            var managedCampaignIds = await GetManagedCampaignIds();

            if (managedCampaignIds.Contains(authorizableEvent.CampaignId))
            {
                return true;
            }
            
            return false;
        }
    }
}
