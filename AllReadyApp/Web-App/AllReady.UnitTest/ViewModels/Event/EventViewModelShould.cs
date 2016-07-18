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
        public void AssignCampaignIdCampaignNameAndTimeZoneId_WhenConstructingWithNonNullCampaign()
        {
            var campaign = new Models.Campaign { Id = 1, Name = "Campaignname", TimeZoneId = "CampaignTimeZoneId" };
            var @event = new Models.Event { Campaign = campaign };
            var sut = new EventViewModel(@event);

            Assert.Equal(sut.CampaignId, campaign.Id);
            Assert.Equal(sut.CampaignName, campaign.Name);
            Assert.Equal(sut.TimeZoneId, campaign.TimeZoneId);
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationIdOrganizationNameAndHasPrivacyPolicy_WhenConstructingEventThatHasANonNullCampaignWithANonNullOrganization()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void CreateNewLocationViewModelAndAssignsToLocation_WhenEventsLocationIsNotNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignIsClosedToTrue_WhenEventsEndDateTimeIsLessThanDateTimeOffsetUtcNow()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignIsClosedToFalse_WhenEventsEndDateTimeIsGreaterThanDateTimeOffsetUtcNow()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void PopulateTasksWithListOfTaskViewModelsInDescendingOrderByStartDateTime_WhenEventsTasksAreNotNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void PopulateTasksWithEmptyListOfTaskViewModels_WhenEventsTasksAreNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void PopulateRequiredSkillsWithListOfSkillViewModelsForEventsRequiredSkills_WhenEventsRequiredSkillsIsNotNull()
        {
        }

        [Fact]
        public void AssignsNullToRequiredSkills_WhenEventsRequiredSkillsIsNull()
        {
        }
    }
}
