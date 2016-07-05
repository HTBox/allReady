using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels.Event;
using Xunit;

namespace AllReady.UnitTest.ViewModels.Event
{
    public class EventViewModelShould
    {
        [Fact]
        public void AssignsCampaignIdCampaignNameAndTimeZoneId_WhenConstructingWithNonNullCampaign()
        {
            var campaign = new Models.Campaign { Id = 1, Name = "Campaignname", TimeZoneId = "CampaignTimeZoneId" };
            var @event = new Models.Event { Campaign = campaign };
            var sut = new EventViewModel(@event);

            Assert.Equal(sut.CampaignId, campaign.Id);
            Assert.Equal(sut.CampaignName, campaign.Name);
            Assert.Equal(sut.TimeZoneId, campaign.TimeZoneId);
        }

        [Fact]
        public void AssignOrganizationIdOrganizationNameAndHasPrivacyPolicy_WhenConstructingEventThatHasANonNullCampaignWithANonNullOrganization()
        {
        }

        [Fact]
        public void CreatesNewLocationViewModelAndAssignsToLocation_WhenEventsLocationIsNotNull()
        {
        }

        [Fact]
        public void AssignsIsClosedToTrueWhen_WhenEventsEndDateTimeIsLessThanDateTimeOffsetUtcNow()
        {
        }

        [Fact]
        public void AssignsIsClosedToWhen_WhenEventsEndDateTimeIsGreaterThanDateTimeOffsetUtcNow()
        {
        }

        [Fact]
        public void PopulatesTasksWithListOfTaskViewModelsInDescendingOrderByStartDateTime_WhenEventsTasksAreNotNull()
        {
        }

        [Fact]
        public void PopulatesTasksWithEmptyListOfTaskViewModels_WhenEventsTasksAreNull()
        {
        }

        [Fact]
        public void PopulatesRequiredSkillsWithListOfSkillViewModelsForEventsRequiredSkills_WhenEventsRequiredSkillsIsNotNull()
        {
        }

        [Fact]
        public void AssignsNullToRequiredSkills_WhenEventsRequiredSkillsIsNull()
        {
        }
    }
}
