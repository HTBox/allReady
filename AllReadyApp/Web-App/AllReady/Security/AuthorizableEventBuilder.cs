using System;
using System.Threading.Tasks;
using AllReady.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AllReady.Security
{
    public interface IAuthorizableEventBuilder
    {
        Task<IAuthorizableEvent> Build(int eventId, int? campaignId = null, int? orgId = null);
    }

    public class AuthorizableEventBuilder : IAuthorizableEventBuilder
    {
        private readonly AllReadyContext _context;
        private readonly IMemoryCache _cache;

        private const string CachePrefix = "AuthorizableEvent_";

        public AuthorizableEventBuilder(AllReadyContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

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

            authorizableEvent = new AuthorizableEvent(eventId, finalCampaignId, finalOrgId);

            _cache.Set(GetCacheKey(eventId), authorizableEvent, TimeSpan.FromHours(1)); // cached for 1 hour

            return authorizableEvent;
        }

        private static bool HasValidIds(int? campaignId, int? orgId)
        {
            if (!campaignId.HasValue || !orgId.HasValue)
            {
                return false;
            }

            if (campaignId.Value == 0 || orgId.Value == 0)
            {
                return false;
            }

            return true;
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
        }

        private class AuthorizableEvent : IAuthorizableEvent
        {
            public AuthorizableEvent(int eventId, int campaignId, int orgId)
            {
                EventId = eventId;
                CampaignId = campaignId;
                OrganizationId = orgId;
            }

            public int EventId { get; }

            public int CampaignId { get; }

            public int OrganizationId { get; }
        }
    }
}
