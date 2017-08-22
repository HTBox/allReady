using System;
using System.Threading.Tasks;
using AllReady.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AllReady.Security
{
    public class AuthorizableTaskBuilder : IAuthorizableTaskBuilder
    {
        private readonly AllReadyContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUserAuthorizationService _userAuthorizationService;

        private const string CachePrefix = "AuthorizableTask_";

        public AuthorizableTaskBuilder(AllReadyContext context, IMemoryCache cache, IUserAuthorizationService userAuthorizationService)
        {
            _context = context;
            _cache = cache;
            _userAuthorizationService = userAuthorizationService;
        }

        /// <inheritdoc />
        public async Task<IAuthorizableTask> Build(int taskId, int? eventId = null, int? campaignId = null, int? orgId = null)
        {
            IAuthorizableTaskIdContainer cacheAuthorizableTaskIdContainer;

            if (taskId == 0)
            {
                return new NotFoundAuthorizableTask();
            }

            if (_cache.TryGetValue(GetCacheKey(taskId), out cacheAuthorizableTaskIdContainer))
            {
                return new AuthorizableTask(cacheAuthorizableTaskIdContainer, _userAuthorizationService);
            }

            int finalEventId;
            int finalCampaignId;
            int finalOrgId;

            if (!HasValidIds(eventId, campaignId, orgId))
            {
                var loadedIds = await
                    _context.VolunteerTasks.AsNoTracking()
                        .Where(x => x.Id == taskId)
                        .Include(x => x.Event).ThenInclude(x => x.Campaign)
                        .Include(x => x.Organization)
                        .FirstOrDefaultAsync();

                if (loadedIds == null)
                {
                    return new NotFoundAuthorizableTask();
                }

                finalEventId = loadedIds.EventId;
                finalCampaignId = loadedIds.Event.CampaignId;
                finalOrgId = loadedIds.Organization.Id;
            }
            else
            {
                finalEventId = eventId.Value;
                finalCampaignId = campaignId.Value;
                finalOrgId = orgId.Value;
            }

            var authorizedEventIdContainer = new AuthorizableTaskIdContainer(taskId, finalEventId, finalCampaignId, finalOrgId);

            _cache.Set(GetCacheKey(taskId), authorizedEventIdContainer, TimeSpan.FromHours(1)); // cached for 1 hour

            return new AuthorizableTask(authorizedEventIdContainer, _userAuthorizationService);
        }

        private class AuthorizableTaskIdContainer : IAuthorizableTaskIdContainer
        {
            public AuthorizableTaskIdContainer(int taskId, int eventId, int campaignId, int orgId)
            {
                TaskId = taskId;
                EventId = eventId;
                CampaignId = campaignId;
                OrganizationId = orgId;
            }

            public int TaskId { get; }
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

        private static string GetCacheKey(int taskId)
        {
            return string.Concat(CachePrefix, taskId.ToString());
        }

        private class NotFoundAuthorizableTask : IAuthorizableTask
        {
            public int TaskId => 0;
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
        private class AuthorizableTask : IAuthorizableTask
        {
            private readonly IUserAuthorizationService _userAuthorizationService;

            public AuthorizableTask(IAuthorizableTaskIdContainer AuthorizableTaskIds, IUserAuthorizationService userAuthorizationService)
            {
                _userAuthorizationService = userAuthorizationService;
                TaskId = AuthorizableTaskIds.TaskId;
                EventId = AuthorizableTaskIds.EventId;
                CampaignId = AuthorizableTaskIds.CampaignId;
                OrganizationId = AuthorizableTaskIds.OrganizationId;
            }

            /// <inheritdoc />
            public int TaskId { get; }

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
                       || userAccessType == ItineraryAccessType.EventAdmin;
            }
        }
    }

    /// <summary>
    /// Defines the possible access types that can potentially manage an <see cref="IAuthorizableTask"/>
    /// </summary>
    public enum TaskAccessType
    {
        Unknown = 0,
        Unauthorized = 1,
        SiteAdmin = 2,
        OrganizationAdmin = 3,
        CampaignAdmin = 4,
        EventAdmin = 5,
    }
}
