﻿using System;
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
        //[Fact(Skip = "NotImplemented")]
        [Fact]
        public void ConstructEventViewModel_WithTheCorrectData()
        {
            var campaign = new Models.Campaign { Id = 1, Name = "Campaignname", TimeZoneId = "CampaignTimeZoneId" };
            var location = new AllReady.ViewModels.Shared.LocationViewModel { City = "Amsterdam" };
            var skills = new List<SkillViewModel>() { new SkillViewModel { Name = "F sharp" } };
            var userSkills = new List<SkillViewModel> { new SkillViewModel { Name = "Medic", Description = "first aid helper", HierarchicalName = "med01", Id = 1 } };
            var signup =  new AllReady.ViewModels.Shared.TaskSignupViewModel() { Name = "task1", TaskId = 1 };
            var @event = new Models.Event { Campaign = campaign, TimeZoneId = "EventTimeZoneId" };
           
            var sut = new EventViewModel(@event);
            var task = new TaskViewModel { CampaignName = sut.CampaignName, CampaignId = sut.CampaignId, eventName = sut.Title, Name = "tasks" };
            var tasks = new List<TaskViewModel> { task };
          
            sut.Description = "Testing the allReady from htbox";
            sut.EndDateTime = DateTimeOffset.Now.AddDays(3650);
            sut.EventType = EventType.Rally;
            sut.HasPrivacyPolicy = false;
            sut.Headline = "the Already test campaing";
            sut.Id = 1;
            sut.ImageUrl = "http://www.htbox.org/";
            sut.IsAllowWaitList = false;
            sut.IsClosed = false;
            sut.IsLimitVolunteers = true;
            sut.Location = location;
            sut.OrganizationId = 1;
            sut.OrganizationName = "TestOrg";
            sut.RequiredSkills = skills;
            sut.SignupModel = signup;
            sut.StartDateTime = DateTimeOffset.Now.AddDays(365);
            sut.Tasks = tasks;
            sut.TimeZoneId = "US Mountain Standard Time";
            sut.Title = "Test Event";
            sut.UserId = "99";
            sut.UserSkills = userSkills;
            sut.UserTasks = tasks;

            Assert.Equal(sut.CampaignId, campaign.Id);
            Assert.Equal(sut.CampaignName, campaign.Name);
            Assert.Equal(sut.Location, location);
            Assert.Equal(sut.UserSkills, userSkills);
            Assert.Equal(sut.UserTasks, tasks);
            Assert.Equal(sut.SignupModel, signup);
            Assert.Equal(sut.RequiredSkills, skills);
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
            var campaign = new Models.Campaign { ManagingOrganization = new Organization { Id = 1, Name = "OrgName" } };
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
            var @event = new Models.Event { Campaign = new Models.Campaign { ManagingOrganization = new Organization { PrivacyPolicy = "PrivacyPolicy" } } };
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
            var @event = new Models.Event { Tasks = new List<AllReadyTask> { new AllReadyTask { StartDateTime = DateTimeOffset.UtcNow.AddDays(2) }, new AllReadyTask { StartDateTime = DateTimeOffset.UtcNow.AddDays(1) } } };
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
            var @event = new Models.Event { RequiredSkills = new List<EventSkill> { new EventSkill { Skill = new Skill() }, new EventSkill { Skill = new Skill() } } };
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