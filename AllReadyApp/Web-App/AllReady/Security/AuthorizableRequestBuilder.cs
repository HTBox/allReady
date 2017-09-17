using System;
using System.Threading.Tasks;
using AllReady.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AllReady.Security
{
    public class AuthorizableRequestBuilder : IAuthorizableRequestBuilder
    {
        private readonly AllReadyContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUserAuthorizationService _userAuthorizationService;

        private const string CachePrefix = "AuthorizableRequest_";

        public AuthorizableRequestBuilder(AllReadyContext context, IMemoryCache cache, IUserAuthorizationService userAuthorizationService)
        {
            _context = context;
            _cache = cache;
            _userAuthorizationService = userAuthorizationService;
        }

        /// <inheritdoc />
        public async Task<IAuthorizableRequest> Build(Guid requestId, int? itineraryId = null, int? eventId = null, int? campaignId = null, int? orgId = null)
        {
            if (requestId == Guid.Empty)
            {
                return new NotFoundAuthorizableRequest();
            }

            if (_cache.TryGetValue(GetCacheKey(requestId), out IAuthorizableRequestIdContainer cacheAuthorizableRequestIdContainer))
            {
                return new AuthorizableRequest(cacheAuthorizableRequestIdContainer, _userAuthorizationService);
            }

            int finalItineraryId;
            int finalEventId;
            int finalCampaignId;
            int finalOrgId;

            if (!HasValidIds(itineraryId, eventId, campaignId, orgId))
            {
                var loadedIds = await
                    _context.Requests.AsNoTracking()
                        .Include(x => x.Itinerary)
                        .ThenInclude(x => x.Itinerary)
                        .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Campaign)
                        .Where(x => x.RequestId == requestId)
                        .Select(x => new { x.ItineraryId, x.EventId, @Event = x.Event, x.OrganizationId })
                        .FirstOrDefaultAsync();

                if (loadedIds == null)
                {
                    return new NotFoundAuthorizableRequest();
                }

                // it's possible for a request to be linked to some/none of these so we use -1 to distinguish those
                finalItineraryId = loadedIds.ItineraryId ?? -1;
                finalEventId = loadedIds.EventId ?? -1;
                finalCampaignId = loadedIds.@Event != null ? loadedIds.@Event.CampaignId : -1;
                finalOrgId = loadedIds.OrganizationId ?? -1;
            }
            else
            {
                finalItineraryId = itineraryId.Value;
                finalEventId = eventId.Value;
                finalCampaignId = campaignId.Value;
                finalOrgId = orgId.Value;
            }

            var authorizedEventIdContainer = new AuthorizableRequestIdContainer(requestId, finalItineraryId, finalEventId, finalCampaignId, finalOrgId);

            _cache.Set(GetCacheKey(requestId), authorizedEventIdContainer, TimeSpan.FromHours(1)); // cached for 1 hour

            return new AuthorizableRequest(authorizedEventIdContainer, _userAuthorizationService);
        }

        private class AuthorizableRequestIdContainer : IAuthorizableRequestIdContainer
        {
            public AuthorizableRequestIdContainer(Guid requestId, int itineraryId, int eventId, int campaignId, int orgId)
            {
                RequestId = requestId;
                ItineraryId = itineraryId;
                EventId = eventId;
                CampaignId = campaignId;
                OrganizationId = orgId;
            }

            public Guid RequestId { get; }
            public int ItineraryId { get; }
            public int EventId { get; }
            public int OrganizationId { get; }
            public int CampaignId { get; }
        }

        private static bool HasValidIds(int? itineraryId, int? eventId, int? campaignId, int? orgId)
        {
            if (!itineraryId.HasValue || !eventId.HasValue || !campaignId.HasValue || !orgId.HasValue)
            {
                return false;
            }

            return itineraryId.Value != 0 && eventId.Value != 0 && campaignId.Value != 0 && orgId.Value != 0;
        }

        private static string GetCacheKey(Guid requestId)
        {
            return string.Concat(CachePrefix, requestId.ToString());
        }

        private class NotFoundAuthorizableRequest : IAuthorizableRequest
        {
            public Guid RequestId => Guid.Empty;
            public int ItineraryId => 0;
            public int CampaignId => 0;
            public int EventId => 0;
            public int OrganizationId => 0;

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
        private class AuthorizableRequest : IAuthorizableRequest
        {
            private readonly IUserAuthorizationService _userAuthorizationService;

            public AuthorizableRequest(IAuthorizableRequestIdContainer AuthorizableRequestIds, IUserAuthorizationService userAuthorizationService)
            {
                _userAuthorizationService = userAuthorizationService;
                RequestId = AuthorizableRequestIds.RequestId;
                ItineraryId = AuthorizableRequestIds.ItineraryId;
                EventId = AuthorizableRequestIds.EventId;
                CampaignId = AuthorizableRequestIds.CampaignId;
                OrganizationId = AuthorizableRequestIds.OrganizationId;
            }

            /// <inheritdoc />
            public Guid RequestId { get; }

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
            public Task<bool> UserCanDelete()
            {
                return UserCanManage();
            }

            /// <inheritdoc />
            public Task<bool> UserCanView()
            {
                return UserCanManage();
            }

            /// <inheritdoc />
            public Task<bool> UserCanEdit()
            {
                return UserCanManage();
            }

            private async Task<bool> UserCanManage()
            {
                var userAccessType = await UserAccessType();

                return userAccessType == ItineraryAccessType.SiteAdmin
                       || userAccessType == ItineraryAccessType.OrganizationAdmin
                       || userAccessType == ItineraryAccessType.CampaignAdmin
                       || userAccessType == ItineraryAccessType.EventAdmin
                       || userAccessType == ItineraryAccessType.TeamLead;
            }
        }
    }

    /// <summary>
    /// Defines the possible access types that can potentially manage an <see cref="IAuthorizableRequest"/>
    /// </summary>
    public enum RequestAccessType
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
