using System;
using System.Collections.Generic;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Task;
using Xunit;

namespace AllReady.UnitTest.ViewModels.Event
{
    public class EventViewModelShould
    {
        //happy path test. set up data to get all possible properties populated when EventViewModel is returned from handler
        [Fact(Skip = "NotImplemented")]
        public void ConstructEventViewModel_WithTheCorrectData()
        {
        }

        [Fact]
        public void SetCampaignIdCampaignNameAndTimeZoneId_WhenConstructingWithNonNullCampaign()
        {
            var campaign = new Models.Campaign { Id = 1, Name = "Campaignname", TimeZoneId = "CampaignTimeZoneId" };
            var @event = new Models.Event { Campaign = campaign, TimeZoneId = "EventTimeZoneId" };
            var sut = new EventViewModel(@event);

            Assert.Equal(sut.CampaignId, campaign.Id);
            Assert.Equal(sut.CampaignName, campaign.Name);
            Assert.Equal(sut.TimeZoneId, @event.TimeZoneId);
        }

        [Fact]
        public void SetOrganizationIdAndOrganizationName_WhenConstructingWithNonNullCampaignAndNonNullManagingOrganization()
        {
            var campaign = new Models.Campaign { ManagingOrganization = new Organization { Id = 1, Name = "OrgName" }};
            var @event = new Models.Event { Campaign = campaign };
            var sut = new EventViewModel(@event);

            Assert.Equal(sut.OrganizationId, campaign.ManagingOrganization.Id);
            Assert.Equal(sut.OrganizationName, campaign.ManagingOrganization.Name);
        }

        [Fact]
        public void SetHasPrivacyPolicyToFalse_WhenEventsCampaignsOrganizationsPrivacyPolicyIsNullOrEmpty_AndConstructingWithNonNullCampaignAndNonNullManagingOrganization()
        {
            var @event = new Models.Event { Campaign = new Models.Campaign { ManagingOrganization = new Organization() } };
            var sut = new EventViewModel(@event);

            Assert.False(sut.HasPrivacyPolicy);
        }

        [Fact]
        public void SetHasPrivacyPolicyToTrue_WhenEventsCampaignsOrganizationsPrivacyPolicyIsNotNullOrEmpty_AndConstructingWithNonNullCampaignAndNonNullManagingOrganization()
        {
            var @event = new Models.Event { Campaign = new Models.Campaign { ManagingOrganization = new Organization { PrivacyPolicy = "PrivacyPolicy" }}};
            var sut = new EventViewModel(@event);

            Assert.True(sut.HasPrivacyPolicy);
        }

        [Fact]
        public void SetLocationToNewLocationViewModelWithCorrectData_WhenEventsLocationIsNotNull_AndConstructingWithNonNullCampaignAndNonNullManagingOrganization()
        {
            var location = new Location { Address1 = "Address1", Address2 = "Address2", City = "City", State = "State", PostalCode = "PostalCode", Country = "Country" };
            var @event = new Models.Event { Location = location };
            var sut = new EventViewModel(@event);

            Assert.Equal(sut.Location.Address1, location.Address1);
            Assert.Equal(sut.Location.Address2, location.Address2);
            Assert.Equal(sut.Location.City, location.City);
            Assert.Equal(sut.Location.State, location.State);
            Assert.Equal(sut.Location.PostalCode, location.PostalCode);
            Assert.Equal(sut.Location.Country, location.Country);
        }

        [Fact]
        public void SetLocationToNull_WhenEventsLocationIstNull_AndConstructingWithNonNullCampaignAndNonNullManagingOrganization()
        {
            var sut = new EventViewModel(new Models.Event());
            Assert.Null(sut.Location);
        }

        [Fact]
        public void SetIsClosedToTrue_WhenEventsEndDateTimeIsLessThanDateTimeOffsetUtcNow()
        {
            var sut = new EventViewModel(new Models.Event { EndDateTime = DateTimeOffset.MinValue });
            Assert.True(sut.IsClosed);
        }

        [Fact]
        public void SetIsClosedToFalse_WhenEventsEndDateTimeIsGreaterThanDateTimeOffsetUtcNow()
        {
            var sut = new EventViewModel(new Models.Event { EndDateTime = DateTimeOffset.MaxValue });
            Assert.False(sut.IsClosed);
        }

        [Fact]
        public void SetTasksToListOfTaskViewModelsInAscendingOrderByStartDateTime_WhenEventsTasksAreNotNull()
        {
            var @event = new Models.Event { Tasks = new List<AllReadyTask> { new AllReadyTask { StartDateTime = DateTimeOffset.UtcNow.AddDays(2)}, new AllReadyTask { StartDateTime = DateTimeOffset.UtcNow.AddDays(1) }}};
            var sut = new EventViewModel(@event);

            Assert.IsType<List<TaskViewModel>>(sut.Tasks);
            Assert.Equal(sut.Tasks.IsOrderedByAscending(x => x.StartDateTime), true);
        }

        [Fact]
        public void SetTasksToEmptyListOfTaskViewModels_WhenEventsTasksAreNull()
        {
            var sut = new EventViewModel(new Models.Event());
            Assert.IsType<List<TaskViewModel>>(sut.Tasks);
            Assert.True(sut.Tasks.Count == 0);
        }

        [Fact]
        public void SetRequiredSkillsToListOfSkillViewModelsForEventsRequiredSkills_WhenEventsRequiredSkillsIsNotNull()
        {
            var @event = new Models.Event { RequiredSkills = new List<EventSkill> { new EventSkill { Skill = new Skill() }, new EventSkill { Skill = new Skill() }}};
            var sut = new EventViewModel(@event);

            Assert.IsType<List<SkillViewModel>>(sut.RequiredSkills);
            Assert.True(sut.RequiredSkills.Count > 0);
        }

        [Fact]
        public void SetRequiredSkillsToNull_WhenEventsRequiredSkillsIsNull()
        {
            var sut = new EventViewModel(new Models.Event { RequiredSkills = null });
            Assert.Null(sut.RequiredSkills);
        }
    }
}