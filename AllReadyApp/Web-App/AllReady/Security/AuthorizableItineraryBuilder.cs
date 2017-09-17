using System;
using System.Threading.Tasks;
using AllReady.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AllReady.Security
{
    public class AuthorizableItineraryBuilder : IAuthorizableItineraryBuilder
    {
        private readonly AllReadyContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUserAuthorizationService _userAuthorizationService;

        private const string CachePrefix = "AuthorizableItinerary_";

        public AuthorizableItineraryBuilder(AllReadyContext context, IMemoryCache cache, IUserAuthorizationService userAuthorizationService)
        {
            _context = context;
            _cache = cache;
            _userAuthorizationService = userAuthorizationService;
        }

        /// <inheritdoc />
        public async Task<IAuthorizableItinerary> Build(int itineraryId, int? eventId = null, int? campaignId = null, int? orgId = null)
        {
            IAuthorizableItineraryIdContainer cacheAuthorizableItineraryIdContainer;

            if (itineraryId == 0)
            {
                return new NotFoundAuthorizableItinerary();
            }

            if (_cache.TryGetValue(GetCacheKey(itineraryId), out cacheAuthorizableItineraryIdContainer))
            {
                return new AuthorizableItinerary(cacheAuthorizableItineraryIdContainer, _userAuthorizationService);
            }

            int finalEventId;
            int finalCampaignId;
            int finalOrgId;

            if (!HasValidIds(eventId, campaignId, orgId))
            {
                var loadedIds = await
                    _context.Itineraries.AsNoTracking()
                        .Include(x => x.Event).ThenInclude(x => x.Campaign)
                        .Where(x => x.Id == itineraryId)
                        .Select(x => new { x.EventId, x.Event.CampaignId, x.Event.Campaign.ManagingOrganizationId })
                        .FirstOrDefaultAsync();

                if (loadedIds == null)
                {
                    return new NotFoundAuthorizableItinerary();
                }

                finalEventId = loadedIds.EventId;
                finalCampaignId = loadedIds.CampaignId;
                finalOrgId = loadedIds.ManagingOrganizationId;
            }
            else
            {
                finalEventId = eventId.Value;
                finalCampaignId = campaignId.Value;
                finalOrgId = orgId.Value;
            }

            var authorizedEventIdContainer = new AuthorizableItineraryIdContainer(itineraryId, finalEventId, finalCampaignId, finalOrgId);

            _cache.Set(GetCacheKey(itineraryId), authorizedEventIdContainer, TimeSpan.FromHours(1)); // cached for 1 hour

            return new AuthorizableItinerary(authorizedEventIdContainer, _userAuthorizationService);
        }

        private class AuthorizableItineraryIdContainer : IAuthorizableItineraryIdContainer
        {
            public AuthorizableItineraryIdContainer(int itineraryId, int eventId, int campaignId, int orgId)
            {
                ItineraryId = itineraryId;
                EventId = eventId;
                CampaignId = campaignId;
                OrganizationId = orgId;
            }

            public int ItineraryId { get; }
            public int EventId { get; }
            public int OrganizationId { get; }
            public int CampaignId { get; }
        }

        private static bool HasValidIds(int? eventId, int? campaignId, int? orgId)
        {
            if (!eventId.HasValue || !campaignId.HasValue || !orgId.HasValue)
            {
                return false;
            }

            return eventId.Value != 0 && campaignId.Value != 0 && orgId.Value != 0;
        }

        private static string GetCacheKey(int itineraryId)
        {
            return string.Concat(CachePrefix, itineraryId.ToString());
        }

        private class NotFoundAuthorizableItinerary : IAuthorizableItinerary
        {
            public int ItineraryId => 0;
            public int CampaignId => 0;
            public int EventId => 0;
            public int OrganizationId => 0;

            public Task<bool> UserCanManageRequests()
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

            public Task<bool> UserCanManageTeamMembers()
            {
                return Task.FromResult(false);
            }
        }

        /// <inheritdoc />
        private class AuthorizableItinerary : IAuthorizableItinerary
        {
            private readonly IUserAuthorizationService _userAuthorizationService;

            public AuthorizableItinerary(IAuthorizableItineraryIdContainer AuthorizableItineraryIds, IUserAuthorizationService userAuthorizationService)
            {
                _userAuthorizationService = userAuthorizationService;
                ItineraryId = AuthorizableItineraryIds.ItineraryId;
                EventId = AuthorizableItineraryIds.EventId;
                CampaignId = AuthorizableItineraryIds.CampaignId;
                OrganizationId = AuthorizableItineraryIds.OrganizationId;
            }

            /// <inheritdoc />
            public int ItineraryId { get; }

            /// <inheritdoc />
            public int EventId { get; }

            /// <inheritdoc />
            public int CampaignId { get; }

            /// <inheritdoc />
            public int OrganizationId { get; }

            private async Task<ItineraryAccessType> UserAccessType()
            {
                if (!_userAuthorizationService.HasAssociatedUser)
                {
                    return ItineraryAccessType.Unauthorized;
                }

                if (_userAuthorizationService.IsSiteAdmin)
                {
                    return ItineraryAccessType.SiteAdmin;
                }

                if (_userAuthorizationService.IsOrganizationAdmin(OrganizationId))
                {
                    return ItineraryAccessType.OrganizationAdmin;
                }

                var managedEventIds = await _userAuthorizationService.GetManagedEventIds();

                if (managedEventIds.Contains(EventId))
                {
                    return ItineraryAccessType.EventAdmin;
                }

                var managedCampaignIds = await _userAuthorizationService.GetManagedCampaignIds();

                if (managedCampaignIds.Contains(CampaignId))
                {
                    return ItineraryAccessType.CampaignAdmin;
                }

                var ledItineraryIds = await _userAuthorizationService.GetLedItineraryIds();

                if (ledItineraryIds.Contains(ItineraryId))
                {
                    return ItineraryAccessType.TeamLead;
                }

                return ItineraryAccessType.Unknown;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanDelete()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == ItineraryAccessType.SiteAdmin
                       || userAccessType == ItineraryAccessType.OrganizationAdmin
                       || userAccessType == ItineraryAccessType.CampaignAdmin
                       || userAccessType == ItineraryAccessType.EventAdmin;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanView()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == ItineraryAccessType.SiteAdmin
                       || userAccessType == ItineraryAccessType.OrganizationAdmin
                       || userAccessType == ItineraryAccessType.CampaignAdmin
                       || userAccessType == ItineraryAccessType.EventAdmin
                       || userAccessType == ItineraryAccessType.TeamLead;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanEdit()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == ItineraryAccessType.SiteAdmin
                       || userAccessType == ItineraryAccessType.OrganizationAdmin
                       || userAccessType == ItineraryAccessType.CampaignAdmin
                       || userAccessType == ItineraryAccessType.EventAdmin;
            }

            /// <inheritdoc />
            public async Task<bool> UserCanManageRequests()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == ItineraryAccessType.SiteAdmin
                       || userAccessType == ItineraryAccessType.OrganizationAdmin
                       || userAccessType == ItineraryAccessType.CampaignAdmin
                       || userAccessType == ItineraryAccessType.EventAdmin
                       || userAccessType == ItineraryAccessType.TeamLead;
            }

            public async Task<bool> UserCanManageTeamMembers()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == ItineraryAccessType.SiteAdmin
                       || userAccessType == ItineraryAccessType.OrganizationAdmin
                       || userAccessType == ItineraryAccessType.CampaignAdmin
                       || userAccessType == ItineraryAccessType.EventAdmin;
            }
        }
    }

    /// <summary>
    /// Defines the possible access types that can potentially manage an <see cref="IAuthorizableItinerary"/>
    /// </summary>
    public enum ItineraryAccessType
    {
        Unknown = 0,
        Unauthorized = 1,
        SiteAdmin = 2,
        OrganizationAdmin = 3,
        CampaignAdmin = 4,
        EventAdmin = 5,
        TeamLead = 6
    }
}
