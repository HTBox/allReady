using System;
using System.Threading.Tasks;
using AllReady.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AllReady.Security
{
    public class AuthorizableEventEventBuilder : IAuthorizableEventBuilder
    {
        private readonly AllReadyContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUserAuthorizationService _userAuthorizationService;

        private const string CachePrefix = "AuthorizableEvent_";

        public AuthorizableEventEventBuilder(AllReadyContext context, IMemoryCache cache, IUserAuthorizationService userAuthorizationService)
        {
            _context = context;
            _cache = cache;
            _userAuthorizationService = userAuthorizationService;
        }

        /// <inheritdoc />
        public async Task<IAuthorizableEvent> Build(int eventId, int? campaignId = null, int? orgId = null)
        {
            IAuthorizableEvent authorizableEvent;

            if (eventId == 0)
            {
                return new NotFoundAuthorizableEvent();
            }

            if (_cache.TryGetValue(GetCacheKey(eventId), out authorizableEvent))
                return authorizableEvent;

            int finalCampaignId;
            int finalOrgId;

            if (!HasValidIds(campaignId, orgId))
            {
                var loadedIds = await
                    _context.Events.AsNoTracking()
                        .Include(x => x.Campaign)
                        .Where(x => x.Id == eventId)
                        .Select(x => new { x.CampaignId, x.Campaign.ManagingOrganizationId })
                        .FirstOrDefaultAsync();

                if (loadedIds == null)
                {
                    return new NotFoundAuthorizableEvent();
                }

                finalCampaignId = loadedIds.CampaignId;
                finalOrgId = loadedIds.ManagingOrganizationId;
            }
            else
            {
                finalCampaignId = campaignId.Value;
                finalOrgId = orgId.Value;
            }

            authorizableEvent = new AuthorizableEvent(eventId, finalCampaignId, finalOrgId, _userAuthorizationService);

            _cache.Set(GetCacheKey(eventId), authorizableEvent, TimeSpan.FromHours(1)); // cached for 1 hour

            return authorizableEvent;
        }

        private static bool HasValidIds(int? campaignId, int? orgId)
        {
            if (!campaignId.HasValue || !orgId.HasValue)
            {
                return false;
            }

            return campaignId.Value != 0 && orgId.Value != 0;
        }

        private static string GetCacheKey(int eventId)
        {
            return string.Concat(CachePrefix, eventId.ToString());
        }

        private class NotFoundAuthorizableEvent : IAuthorizableEvent
        {
            public int CampaignId => 0;
            public int EventId => 0;
            public int OrganizationId => 0;

            public Task<EventAccessType> UserAccessType()
            {
                return Task.FromResult(EventAccessType.Unauthorized);
            }

            public Task<bool> IsUserAuthorized()
            {
                return Task.FromResult(false);
            }
        }

        private class AuthorizableEvent : IAuthorizableEvent
        {
            private readonly IUserAuthorizationService _userAuthorizationService;

            public AuthorizableEvent(int eventId, int campaignId, int orgId, IUserAuthorizationService userAuthorizationService)
            {
                _userAuthorizationService = userAuthorizationService;
                EventId = eventId;
                CampaignId = campaignId;
                OrganizationId = orgId;
            }

            /// <inheritdoc />
            public int EventId { get; }

            /// <inheritdoc />
            public int CampaignId { get; }

            /// <inheritdoc />
            public int OrganizationId { get; }

            /// <inheritdoc />
            public async Task<EventAccessType> UserAccessType()
            {
                if (!_userAuthorizationService.HasAssociatedUser)
                {
                    return EventAccessType.Unauthorized;
                }

                if (_userAuthorizationService.IsSiteAdmin)
                {
                    return EventAccessType.SiteAdmin;
                }

                if (_userAuthorizationService.IsOrganizationAdmin(OrganizationId))
                {
                    return EventAccessType.OrganizationAdmin;
                }

                var managedEventIds = await _userAuthorizationService.GetManagedEventIds();

                if (managedEventIds.Contains(EventId))
                {
                    return EventAccessType.EventAdmin;
                }

                var managedCampaignIds = await _userAuthorizationService.GetManagedCampaignIds();

                if (managedCampaignIds.Contains(CampaignId))
                {
                    return EventAccessType.CampaignAdmin;
                }

                return EventAccessType.Unknown;
            }

            /// <inheritdoc />
            public async Task<bool> IsUserAuthorized()
            {
                var userAccessType =  await UserAccessType();

                return userAccessType == EventAccessType.SiteAdmin 
                    || userAccessType == EventAccessType.OrganizationAdmin 
                    || userAccessType == EventAccessType.CampaignAdmin 
                    || userAccessType == EventAccessType.EventAdmin;
            }
        }
    }

    /// <summary>
    /// Defines the possible access types that can potentially manage an <see cref="IAuthorizableEvent"/>
    /// </summary>
    public enum EventAccessType
    {
        Unknown = 0,
        Unauthorized = 1,
        SiteAdmin = 2,
        OrganizationAdmin = 3,
        CampaignAdmin = 4,
        EventAdmin = 5
    }
}
