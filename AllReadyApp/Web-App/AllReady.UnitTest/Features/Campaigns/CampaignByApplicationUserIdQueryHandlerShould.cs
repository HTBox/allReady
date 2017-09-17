using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class CampaignByApplicationUserIdQueryHandlerShould : InMemoryContextTest
    {
        private readonly CampaignByApplicationUserIdQuery message;
        private readonly Campaign campaign;
        private readonly CampaignByApplicationUserIdQueryHandler sut;

        public CampaignByApplicationUserIdQueryHandlerShould()
        {
            message = new CampaignByApplicationUserIdQuery() { ApplicationUserId = Guid.NewGuid().ToString() };
            campaign = new Campaign {  Organizer  =  new ApplicationUser() { Id = message.ApplicationUserId }, Published = true };

            Context.Add(campaign);
            Context.SaveChanges();

            sut = new CampaignByApplicationUserIdQueryHandler(Context);
        }

        [Fact]
        public async Task ReturnCorrectAmount()
        {
            var result = await sut.Handle(message);
            Assert.Single(result);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);
            Assert.Same(campaign, result.First());
        }

        [Fact]
        public async Task ReturnCorrectType()
        {
            var result = await sut.Handle(message);
            Assert.IsType<Campaign>(result.First());
        }
    }
}
