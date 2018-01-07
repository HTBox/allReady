using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    class CreateCampaignManagerCommandHandlerShould : InMemoryContextTest
    {
        private string _userId = Guid.NewGuid().ToString();
        private int _campaignId = 10;

        [Fact]
        public async Task AddCampaignManager()
        {
            var handler = new CreateCampaignManagerCommandHandler(Context);
            await handler.Handle(new CreateCampaignManagerCommand
            {
                CampaignId = _campaignId,
                UserId = _userId
            });

            Context.CampaignManagers.Count().ShouldBe(1);
            Context.CampaignManagers.First().CampaignId.ShouldBe(_campaignId);
            Context.CampaignManagers.First().UserId.ShouldBe(_userId);
        }

        public async Task NotAddCampaignManagerIfAllreadyExist()
        {
            Context.CampaignManagers.Add(new CampaignManager
            {
                CampaignId = _campaignId,
                UserId = _userId
            });

            Context.SaveChanges();

            Context.CampaignManagers.Count().ShouldBe(1);

            var handler = new CreateCampaignManagerCommandHandler(Context);
            await handler.Handle(new CreateCampaignManagerCommand
            {
                CampaignId = _campaignId,
                UserId = _userId
            });

            Context.CampaignManagers.Count().ShouldBe(1);
        }
    }
}
