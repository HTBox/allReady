using AllReady.Models;
using AllReady.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using System;

namespace AllReady.UnitTest.Security
{
    public class AuthorizableTaskBuilderTests : InMemoryContextTest
    {
        public static int TaskId = 123;
        public static int TaskIdForTaskWithoutLinks = 764;
        public static int TaskIdForTaskWithPartialLinks = 343;

        [Fact]
        public async Task Build_WithAllFivesIds_ReturnsTheAuthorizableTask()
        {
            var sut = new AuthorizableTaskBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(TaskId, 3, 4, 5);

            result.TaskId.ShouldBe(TaskId);
            result.EventId.ShouldBe(3);
            result.CampaignId.ShouldBe(4);
            result.OrganizationId.ShouldBe(5);
        }

        [Fact]
        public async Task Build_WithJustTaskId_ReturnsFromTheCacheWhenAvailable()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("AuthorizableTask_" + TaskId, new FakeAuthorizableTaskIdContainer(), TimeSpan.FromMinutes(5));

            var sut = new AuthorizableTaskBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(TaskId);

            result.TaskId.ShouldBe(TaskId);
            result.EventId.ShouldBe(200);
            result.CampaignId.ShouldBe(300);
            result.OrganizationId.ShouldBe(400);
        }

        [Fact]
        public async Task Build_WithJustTaskId_ReturnsTheAuthorizableTaskFromTheDatabase()
        {
            var sut = new AuthorizableTaskBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(TaskId);

            result.TaskId.ShouldBe(TaskId);
            result.EventId.ShouldBe(10);
            result.CampaignId.ShouldBe(20);
            result.OrganizationId.ShouldBe(30);
        }

        [Fact]
        public async Task Build_SetsCaches_WithTheAuthorizableTask()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new AuthorizableTaskBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(TaskId);

            var foundInCache = cache.TryGetValue("AuthorizableTask_" + TaskId, out IAuthorizableTaskIdContainer AuthorizableTask);

            foundInCache.ShouldBeTrue();
            AuthorizableTask.TaskId.ShouldBe(TaskId);
            AuthorizableTask.EventId.ShouldBe(10);
            AuthorizableTask.CampaignId.ShouldBe(20);
            AuthorizableTask.OrganizationId.ShouldBe(30);
        }

        private class FakeAuthorizableTaskIdContainer : IAuthorizableTaskIdContainer
        {
            public int TaskId => AuthorizableTaskBuilderTests.TaskId;

            public int ItineraryId => 100;

            public int EventId => 200;

            public int CampaignId => 300;

            public int OrganizationId => 400;
        }

        protected override void LoadTestData()
        {
            var org = new Organization { Id = 30 };

            Context.Organizations.Add(org);

            var campaign = new Campaign { Id = 20, ManagingOrganization = org };

            Context.Campaigns.Add(campaign);

            var @event = new Event { Id = 10, Campaign = campaign };

            Context.Events.Add(@event);

            var task = new VolunteerTask { Id = TaskId, EventId = 10, Organization = org, Event = @event };
            var taskWithoutLinks = new VolunteerTask { Id = TaskIdForTaskWithoutLinks, Organization = new Organization(), Event = new Event { Campaign = new Campaign() } };
            var taskWithPartialLinks = new VolunteerTask { Id = TaskIdForTaskWithPartialLinks, Organization = org, Event = new Event { Campaign = new Campaign() } };

            Context.VolunteerTasks.Add(task);
            Context.VolunteerTasks.Add(taskWithoutLinks);
            Context.VolunteerTasks.Add(taskWithPartialLinks);

            Context.SaveChanges();
        }
    }
}
