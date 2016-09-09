using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var campaign = new Campaign
            {
                Id = 111,
                ManagingOrganization = new Organization()
            };

            Context.Campaigns.Add(campaign);

            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnNull_WhenCampaignIsNotFound()
        {
            var handler = new CampaignSummaryQueryHandler(this.Context);

            var result = await handler.Handle(new CampaignSummaryQuery { CampaignId = 222 });

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReturnCampaignSummaryViewModel_WhenCampaignExists()
        {
            var handler = new CampaignSummaryQueryHandler(this.Context);

            var result = await handler.Handle(new CampaignSummaryQuery { CampaignId = 111 });

            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task SetPrimaryContactInCampaignSummaryViewModel()
        {
            Context.Campaigns.First().CampaignContacts = new List<CampaignContact> {
                new CampaignContact
                {
                    ContactType = (int)ContactTypes.Primary,
                    Contact = new Contact { Email = "contact1@example.com", FirstName="FirstName1",LastName="LastName1",PhoneNumber="111-1111-1111" }
                }
            };

            Context.SaveChanges();

            var handler = new CampaignSummaryQueryHandler(this.Context);

            var result = await handler.Handle(new CampaignSummaryQuery { CampaignId = 111 });

            result.PrimaryContactFirstName.ShouldNotBeNullOrEmpty();
            result.PrimaryContactLastName.ShouldNotBeNullOrEmpty();
            result.PrimaryContactEmail.ShouldNotBeNullOrEmpty();
            result.PrimaryContactPhoneNumber.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task NotSetPrimaryContactInCampaignSummaryViewModel()
        {
            Context.Campaigns.First().CampaignContacts = new List<CampaignContact> {
                new CampaignContact
                {
                    Contact = new Contact { Email = "contact1@example.com", FirstName="FirstName1",LastName="LastName1",PhoneNumber="111-1111-1111" }
                }
            };

            Context.SaveChanges();

            var handler = new CampaignSummaryQueryHandler(this.Context);

            var result = await handler.Handle(new CampaignSummaryQuery { CampaignId = 111 });

            result.PrimaryContactFirstName.ShouldBeNullOrEmpty();
            result.PrimaryContactLastName.ShouldBeNullOrEmpty();
            result.PrimaryContactEmail.ShouldBeNullOrEmpty();
            result.PrimaryContactPhoneNumber.ShouldBeNullOrEmpty();
        }
    }
}
