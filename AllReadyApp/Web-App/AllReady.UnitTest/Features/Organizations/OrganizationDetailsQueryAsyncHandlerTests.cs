﻿using AllReady.Features.Organizations;
using System.Threading.Tasks;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Features.Organizations
{
    public class OrganizationDetailsQueryHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task CorrectOrganizationReturnedWhenIdInMessage()
        {
            var handler = new OrganizationDetailsQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganizationDetailsQueryAsync { Id = 1 });

            Assert.NotNull(result);
            Assert.Equal("Org 1", result.Name);
        }

        [Fact]
        public async Task LockedCampaignsAreNotIncludedInTheResults()
        {
            var handler = new OrganizationDetailsQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganizationDetailsQueryAsync { Id = 1 });

            Assert.NotNull(result);
            Assert.Equal(3, result.Campaigns.Count);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdDoesNotExists()
        {
            var handler = new OrganizationDetailsQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganizationDetailsQueryAsync { Id = 100 });

            Assert.Null(result);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdNotInMessage()
        {
            var handler = new OrganizationDetailsQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganizationDetailsQueryAsync());

            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            OrganizationHandlerTestHelper.LoadOrganizationHandlerTestData(Context);
        }
    }
}