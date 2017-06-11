using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class CampaignDetailQueryHandlerShould : InMemoryContextTest
    {
        private int _campaignId;

        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>(),
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
            };
            firePrev.ManagementInvites = new List<CampaignManagerInvite>
            {
                new CampaignManagerInvite
                {
                    Id = 1,
                    InviteeEmailAddress = "test@test.com",
                    SentDateTimeUtc = new DateTime(2015, 5, 28),
                    CampaignId = _campaignId,
                }
            };
            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.SaveChanges();

            _campaignId = firePrev.Id;
        }

        [Fact]
        public async Task CampaignExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = _campaignId };
            var handler = new CampaignDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = 0 };
            var handler = new CampaignDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnCampaignManagerInvitesWithStatusPending_WhenNotRejectedAcceptedOrRevoked()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = _campaignId };
            var handler = new CampaignDetailQueryHandler(context);
            CampaignDetailViewModel result = await handler.Handle(query);

            result.CampaignManagerInvites.Count().ShouldBe(1);
            result.CampaignManagerInvites.Single().Id.ShouldBe(1);
            result.CampaignManagerInvites.Single().InviteeEmail.ShouldBe("test@test.com");
            result.CampaignManagerInvites.Single().Status.ShouldBe(CampaignDetailViewModel.CampaignManagerInviteStatus.Pending);
        }

        [Fact]
        public async Task ReturnCampaignManagerInvitesWithStatusAccepted_WhenInviteIsAccepted()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = _campaignId };
            var handler = new CampaignDetailQueryHandler(context);
            CampaignManagerInvite invite = context.CampaignManagerInvites.Where(c => c.CampaignId == _campaignId).Single();
            invite.AcceptedDateTimeUtc = new DateTime(2015, 5, 29);
            context.SaveChanges();

            CampaignDetailViewModel result = await handler.Handle(query);

            result.CampaignManagerInvites.Count().ShouldBe(1);
            result.CampaignManagerInvites.Single().Id.ShouldBe(1);
            result.CampaignManagerInvites.Single().InviteeEmail.ShouldBe("test@test.com");
            result.CampaignManagerInvites.Single().Status.ShouldBe(CampaignDetailViewModel.CampaignManagerInviteStatus.Accepted);
        }

        [Fact]
        public async Task ReturnCampaignManagerInvitesWithStatusRejected_WhenInviteIsRejected()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = _campaignId };
            var handler = new CampaignDetailQueryHandler(context);
            CampaignManagerInvite invite = context.CampaignManagerInvites.Where(c => c.CampaignId == _campaignId).Single();
            invite.RejectedDateTimeUtc = new DateTime(2015, 5, 29);
            context.SaveChanges();

            CampaignDetailViewModel result = await handler.Handle(query);

            result.CampaignManagerInvites.Count().ShouldBe(1);
            result.CampaignManagerInvites.Single().Id.ShouldBe(1);
            result.CampaignManagerInvites.Single().InviteeEmail.ShouldBe("test@test.com");
            result.CampaignManagerInvites.Single().Status.ShouldBe(CampaignDetailViewModel.CampaignManagerInviteStatus.Rejected);
        }

        [Fact]
        public async Task ReturnCampaignManagerInvitesWithStatusRevoked_WhenInviteIsRevoked()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = _campaignId };
            var handler = new CampaignDetailQueryHandler(context);
            CampaignManagerInvite invite = context.CampaignManagerInvites.Where(c => c.CampaignId == _campaignId).Single();
            invite.RevokedDateTimeUtc = new DateTime(2015, 5, 29);
            context.SaveChanges();

            CampaignDetailViewModel result = await handler.Handle(query);

            result.CampaignManagerInvites.Count().ShouldBe(1);
            result.CampaignManagerInvites.Single().Id.ShouldBe(1);
            result.CampaignManagerInvites.Single().InviteeEmail.ShouldBe("test@test.com");
            result.CampaignManagerInvites.Single().Status.ShouldBe(CampaignDetailViewModel.CampaignManagerInviteStatus.Revoked);
        }
    }
}