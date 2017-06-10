using AllReady.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace AllReady.Security
{
    public class AuthorizableOrganizationBuilder : IAuthorizableOrganizationBuilder
    {
        private readonly AllReadyContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUserAuthorizationService _userAuthorizationService;

        private const string CachePrefix = "AuthorizableOrganization_";

        public AuthorizableOrganizationBuilder(AllReadyContext context, IMemoryCache cache, IUserAuthorizationService userAuthorizationService)
        {
            _context = context;
            _cache = cache;
            _userAuthorizationService = userAuthorizationService;
        }

        /// <inheritdoc />
        public async Task<IAuthorizableOrganization>
            Build(int organizationId)
        {
            IAuthorizableOrganizationIdContainer cacheAuthorizableOrganizationIdContainer;

            if (organizationId == 0)
            {
                return new NotFoundAuthorizableOrganization();
            }

            if (_cache.TryGetValue(GetCacheKey(organizationId), out cacheAuthorizableOrganizationIdContainer))
            {
                return new AuthorizableOrganization(cacheAuthorizableOrganizationIdContainer, _userAuthorizationService);
            }

            var authorizedOrganizationIdContainer = new AuthorizableOrganizationIdContainer(organizationId);

            _cache.Set(GetCacheKey(organizationId), authorizedOrganizationIdContainer, TimeSpan.FromHours(1)); // cached for 1 hour

            return await Task.Run(() => new AuthorizableOrganization(authorizedOrganizationIdContainer, _userAuthorizationService));
        }

        private class AuthorizableOrganizationIdContainer : IAuthorizableOrganizationIdContainer
        {
            public AuthorizableOrganizationIdContainer(int organizationId)
            {
                OrganizationId = organizationId;
            }

            public int OrganizationId { get; }
        }

        private static string GetCacheKey(int organizationId)
        {
            return string.Concat(CachePrefix, organizationId.ToString());
        }

        private class NotFoundAuthorizableOrganization : IAuthorizableOrganization
        {
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
        private class AuthorizableOrganization : IAuthorizableOrganization
        {
            private readonly IUserAuthorizationService _userAuthorizationService;

            public AuthorizableOrganization(IAuthorizableOrganizationIdContainer authorizableOrganizationIds, IUserAuthorizationService userAuthorizationService)
            {
                _userAuthorizationService = userAuthorizationService;
                OrganizationId = authorizableOrganizationIds.OrganizationId;
            }

            /// <inheritdoc />
            public int OrganizationId { get; }

            private async Task<OrganizationAccessType> UserAccessType()
            {
                if (!_userAuthorizationService.HasAssociatedUser)
                {
                    return OrganizationAccessType.Unauthorized;
                }

                if (_userAuthorizationService.IsSiteAdmin)
                {
                    return OrganizationAccessType.SiteAdmin;
                }

                if (_userAuthorizationService.IsOrganizationAdmin(OrganizationId))
                {
                    return OrganizationAccessType.OrganizationAdmin;
                }

                return await Task.Run(() => OrganizationAccessType.Unknown);
            }

            /// <inheritdoc />
            public async Task<bool> UserCanDelete()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == OrganizationAccessType.SiteAdmin
                       || userAccessType == OrganizationAccessType.OrganizationAdmin;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanView()
            {
                return await UserCanManageOrganization();
            }

            /// <inheritdoc />
            public async Task<bool> UserCanEdit()
            {
                return await UserCanManageOrganization();
            }

            private async Task<bool> UserCanManageOrganization()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == OrganizationAccessType.SiteAdmin
                       || userAccessType == OrganizationAccessType.OrganizationAdmin;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanManageChildObjects()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == OrganizationAccessType.SiteAdmin
                       || userAccessType == OrganizationAccessType.OrganizationAdmin;
            }
        }
    }

    /// <summary>
    /// Defines the possible access types that can potentially manage an <see cref="IAuthorizableOrganization"/>
    /// </summary>
    public enum OrganizationAccessType
    {
        Unknown = 0,
        Unauthorized = 1,
        SiteAdmin = 2,
        OrganizationAdmin = 3
    }
}

