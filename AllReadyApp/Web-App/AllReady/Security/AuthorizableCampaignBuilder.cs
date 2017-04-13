using AllReady.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Security
{
    public class AuthorizableCampaignBuilder : IAuthorizableCampaignBuilder
    {
        private readonly AllReadyContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUserAuthorizationService _userAuthorizationService;

        private const string CachePrefix = "AuthorizableCampaign_";

        public AuthorizableCampaignBuilder(AllReadyContext context, IMemoryCache cache, IUserAuthorizationService userAuthorizationService)
        {
            _context = context;
            _cache = cache;
            _userAuthorizationService = userAuthorizationService;
        }

        /// <inheritdoc />
        public async Task<IAuthorizableCampaign> Build(int campaignId, int? orgId = null)
        {
            IAuthorizableCampaignIdContainer cacheAuthorizableCampaignIdContainer;

            if (campaignId == 0)
            {
                return new NotFoundAuthorizableCampaign();
            }

            if (_cache.TryGetValue(GetCacheKey(campaignId), out cacheAuthorizableCampaignIdContainer))
            {
                return new AuthorizableCampaign(cacheAuthorizableCampaignIdContainer, _userAuthorizationService);
            }

            int finalOrgId;

            if (!HasValidIds(orgId))
            {
                var loadedIds = await
                    _context.Campaigns.AsNoTracking()
                        .Where(x => x.Id == campaignId)
                        .Select(x => new { x.ManagingOrganizationId })
                        .FirstOrDefaultAsync();

                if (loadedIds == null)
                {
                    return new NotFoundAuthorizableCampaign();
                }

                finalOrgId = loadedIds.ManagingOrganizationId;
            }
            else
            {
                finalOrgId = orgId.Value;
            }

            var authorizedCampaignIdContainer = new AuthorizableCampaignIdContainer(campaignId, finalOrgId);

            _cache.Set(GetCacheKey(campaignId), authorizedCampaignIdContainer, TimeSpan.FromHours(1)); // cached for 1 hour

            return new AuthorizableCampaign(authorizedCampaignIdContainer, _userAuthorizationService);
        }

        private class AuthorizableCampaignIdContainer : IAuthorizableCampaignIdContainer
        {
            public AuthorizableCampaignIdContainer(int campaignId, int orgId)
            {
                CampaignId = campaignId;
                OrganizationId = orgId;
            }

            public int OrganizationId { get; }
            public int CampaignId { get; }
        }

        private static bool HasValidIds(int? orgId)
        {
            if (!orgId.HasValue)
            {
                return false;
            }

            return orgId.Value != 0;
        }

        private static string GetCacheKey(int campaignId)
        {
            return string.Concat(CachePrefix, campaignId.ToString());
        }

        private class NotFoundAuthorizableCampaign : IAuthorizableCampaign
        {
            public int CampaignId => 0;
            public int OrganizationId => 0;

            public Task<bool> UserCanManageChildObjects()
            {
                return Task.FromResult(false);
            }

            public Task<bool> UserCanView()
            {
                return Task.FromResult(false);
            }

            public Task<bool> UserCanEdit()
            {
                return Task.FromResult(false);
            }

            public Task<bool> UserCanDelete()
            {
                return Task.FromResult(false);
            }
        }

        /// <inheritdoc />
        private class AuthorizableCampaign : IAuthorizableCampaign
        {
            private readonly IUserAuthorizationService _userAuthorizationService;

            public AuthorizableCampaign(IAuthorizableCampaignIdContainer authorizableCampaignIds, IUserAuthorizationService userAuthorizationService)
            {
                _userAuthorizationService = userAuthorizationService;
                CampaignId = authorizableCampaignIds.CampaignId;
                OrganizationId = authorizableCampaignIds.OrganizationId;
            }

            /// <inheritdoc />
            public int CampaignId { get; }

            /// <inheritdoc />
            public int OrganizationId { get; }

            private async Task<CampaignAccessType> UserAccessType()
            {
                if (!_userAuthorizationService.HasAssociatedUser)
                {
                    return CampaignAccessType.Unauthorized;
                }

                if (_userAuthorizationService.IsSiteAdmin)
                {
                    return CampaignAccessType.SiteAdmin;
                }

                if (_userAuthorizationService.IsOrganizationAdmin(OrganizationId))
                {
                    return CampaignAccessType.OrganizationAdmin;
                }

                var managedCampaignIds = await _userAuthorizationService.GetManagedCampaignIds();

                if (managedCampaignIds.Contains(CampaignId))
                {
                    return CampaignAccessType.CampaignAdmin;
                }

                return CampaignAccessType.Unknown;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanDelete()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == CampaignAccessType.SiteAdmin
                       || userAccessType == CampaignAccessType.OrganizationAdmin;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanView()
            {
                return await UserCanManageCampaign();
            }

            /// <inheritdoc />
            public async Task<bool> UserCanEdit()
            {
                return await UserCanManageCampaign();
            }

            private async Task<bool> UserCanManageCampaign()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == CampaignAccessType.SiteAdmin
                       || userAccessType == CampaignAccessType.OrganizationAdmin
                       || userAccessType == CampaignAccessType.CampaignAdmin;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanManageChildObjects()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == CampaignAccessType.SiteAdmin
                       || userAccessType == CampaignAccessType.OrganizationAdmin
                       || userAccessType == CampaignAccessType.CampaignAdmin;
            }
        }
    }

    /// <summary>
    /// Defines the possible access types that can potentially manage an <see cref="IAuthorizableCampaign"/>
    /// </summary>
    public enum CampaignAccessType
    {
        Unknown = 0,
        Unauthorized = 1,
        SiteAdmin = 2,
        OrganizationAdmin = 3,
        CampaignAdmin = 4
    }
}

