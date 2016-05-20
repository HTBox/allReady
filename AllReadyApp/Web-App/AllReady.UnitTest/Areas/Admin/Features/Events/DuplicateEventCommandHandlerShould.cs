using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class DuplicateEventCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CreateANewEventEntity()
        {
            var sut = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });

            Assert.Equal(2, sut.Id);
        }

        [Fact]
        public async Task CopyEventPropertyValuesToTheNewEvent()
        {
            var duplicateEventModel = new DuplicateEventModel()
            {
                Id = EVENT_TO_DUPLICATE_ID,
                Name = "Name",
                Description = "Description",
                StartDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan()),
                EndDateTime = new DateTimeOffset(2016, 1, 31, 0, 0, 0, new TimeSpan())
            };

            var sut = await DuplicateEvent(duplicateEventModel);

            Assert.Equal(1, sut.CampaignId);
            Assert.Equal(1, sut.Campaign.Id);
            Assert.Equal("Name", sut.Name);
            Assert.Equal("Description", sut.Description);
            Assert.Equal(EventTypes.ItineraryManaged, sut.EventType);
            Assert.Equal(10, sut.NumberOfVolunteersRequired);
            Assert.Equal(new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan()), sut.StartDateTime);
            Assert.Equal(new DateTimeOffset(2016, 1, 31, 0, 0, 0, new TimeSpan()), sut.EndDateTime);
            Assert.Equal("Address1", sut.Location.Address1);
            Assert.Equal("Address2", sut.Location.Address2);
            Assert.Equal("City", sut.Location.City);
            Assert.Equal("State", sut.Location.State);
            Assert.Equal("PostalCode", sut.Location.PostalCode);
            Assert.Equal("Name", sut.Location.Name);
            Assert.Equal("PhoneNumber", sut.Location.PhoneNumber);
            Assert.Equal("Country", sut.Location.Country);
            Assert.Equal(3, sut.Tasks.Count());
            Assert.Equal(0, sut.UsersSignedUp.Count());
            Assert.Equal("Organizer", sut.Organizer.Id);
            Assert.Equal("ImageUrl", sut.ImageUrl);
            // Todo: Implement Copying Required Skills
            //Assert.Equal(_EventToDuplicate.RequiredSkills.Select(s => s.Skill.Id), sut.RequiredSkills.Select(s => s.Skill.Id));
            //Assert.Equal(3, sut.RequiredSkills.Count());
            Assert.Equal(false, sut.IsLimitVolunteers);
            Assert.Equal(true, sut.IsAllowWaitList);
        }

        [Fact(Skip = "NotImplemented")]
        public void CreateANewLocationEntity()
        {
            Event _EventToDuplicate = new Event();
            Event _NewEvent = new Event();
            Assert.False(_EventToDuplicate.Location.Id == _NewEvent.Location.Id);
        }

        [Fact(Skip = "NotImplemented")]
        public void CreateNewTaskEntities()
        {
            // Todo: Improve this:
            // - Linqify
            // - Collection Equivalence
            // - Note: Equals and NotEquals assertions require expected values, 
            //         however in this case, we don't know. Well we do under test,
            //         but that would make the test brittle and dependant on the current test setup.
            Event _EventToDuplicate = new Event();
            Event _NewEvent = new Event();
            for (int i = 0; i < _EventToDuplicate.Tasks.Count; i++)
            {
                Assert.False(_EventToDuplicate.Tasks[i].Id == _NewEvent.Tasks[i].Id);
            }
        }

        [Fact(Skip = "NotImplemented")]
        public void MaintainOffsetBetweenTaskStartTimeAndEventStartTimeInNewTask()
        {
            Event _EventToDuplicate = new Event();
            Event _NewEvent = new Event();
            var offsetsBetweenTaskStartTimeAndEventStartTimeForTaskToDuplicate =
                _EventToDuplicate.Tasks.Select(t => t.StartDateTime - _EventToDuplicate.StartDateTime);

            var offsetsBetweenTaskStartTimeAndEventStartTimeForNewTask =
                _NewEvent.Tasks.Select(t => t.StartDateTime - _NewEvent.StartDateTime);

            Assert.Equal(offsetsBetweenTaskStartTimeAndEventStartTimeForTaskToDuplicate,
                offsetsBetweenTaskStartTimeAndEventStartTimeForNewTask);
        }

        [Fact(Skip = "NotImplemented")]
        public void MaintainTaskDurationInNewTask()
        {
            Event _EventToDuplicate = new Event();
            Event _NewEvent = new Event();
            var eventToDuplicateTaskDurations = _EventToDuplicate.Tasks.Select(t => t.EndDateTime - t.StartDateTime);
            var newEventTaskDurations = _NewEvent.Tasks.Select(t => t.EndDateTime - t.StartDateTime);

            Assert.Equal(eventToDuplicateTaskDurations, newEventTaskDurations);
        }

        #region Helpers
        const int EVENT_TO_DUPLICATE_ID = 1;

        async Task<Event> DuplicateEvent(DuplicateEventModel duplicateEventModel)
        {
            var command = new DuplicateEventCommand() { DuplicateEventModel = duplicateEventModel };
            var handler = new DuplicateEventCommandHandler(Context);
            var responseResult = await handler.Handle(command);
            return Context.Events.Single(e => e.Id == responseResult);
        }

        Event GetEventToDuplicate()
        {
            return Context.Events.Single(e => e.Id == 1);
        }

        protected override void LoadTestData()
        {
            var campaign = new Campaign() { Id = 1 };

            var tasks = new List<AllReadyTask>()
                {
                    new AllReadyTask(),
                    new AllReadyTask(),
                    new AllReadyTask()
                };

            var @event = new Event()
            {
                CampaignId = 1,
                Campaign = campaign,
                Name = "Name",
                Description = "Description",
                EventType = EventTypes.ItineraryManaged,
                NumberOfVolunteersRequired = 10,
                StartDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan()),
                EndDateTime = new DateTimeOffset(2016, 1, 31, 0, 0, 0, new TimeSpan()),
                Location = new Location()
                {
                    Id = 1,
                    Address1 = "Address1",
                    Address2 = "Address2",
                    City = "City",
                    State = "State",
                    PostalCode = "PostalCode",
                    Name = "Name",
                    PhoneNumber = "PhoneNumber",
                    Country = "Country"
                },
                Tasks = tasks,
                UsersSignedUp = new List<EventSignup>()
                {
                    new EventSignup() { Id = 1 },
                    new EventSignup() { Id = 2 },
                    new EventSignup() { Id = 3 }
                },
                Organizer = new ApplicationUser() { Id = "Organizer" },
                ImageUrl = "ImageUrl",
                RequiredSkills = new List<EventSkill>()
                {
                    new EventSkill() { SkillId = 1, Skill = new Skill() { Id = 1 } },
                    new EventSkill() { SkillId = 2, Skill = new Skill() { Id = 2 } },
                    new EventSkill() { SkillId = 3, Skill = new Skill() { Id = 3 } }
                },
                IsLimitVolunteers = false,
                IsAllowWaitList = true
            };
            Context.Add(campaign);
            Context.Add(@event);
            Context.AddRange(tasks);
            Context.SaveChanges();

        }
        #endregion
    }
}
