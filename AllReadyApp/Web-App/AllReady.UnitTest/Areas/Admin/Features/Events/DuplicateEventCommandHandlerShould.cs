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
        Event _EventToDuplicate;
        Event _NewEvent;

        public DuplicateEventCommandHandlerShould()
        {
            _EventToDuplicate = TestEvent();
            Context.Add(_EventToDuplicate);
            Context.SaveChanges();

            var command = new DuplicateEventCommand() { DuplicateEventModel = new DuplicateEventModel() { Id = _EventToDuplicate.Id } };
            var handler = new DuplicateEventCommandHandler(Context);
            var responseResult = handler.Handle(command).Result;

            _NewEvent = Context.Events.Single(e => e.Id == responseResult);
        }

        [Fact]
        public void CreateANewEventEntity()
        {
            Assert.False(_EventToDuplicate.Id == _NewEvent.Id);
        }

        [Fact]
        public void CopyEventPropertyValuesToTheNewEvent()
        {
            Assert.Equal(_EventToDuplicate.CampaignId, _NewEvent.CampaignId);
            Assert.Equal(_EventToDuplicate.Campaign.Id, _NewEvent.Campaign.Id);
            Assert.Equal(_EventToDuplicate.Name, _NewEvent.Name);
            Assert.Equal(_EventToDuplicate.Description, _NewEvent.Description);
            Assert.Equal(_EventToDuplicate.EventType, _NewEvent.EventType);
            Assert.Equal(_EventToDuplicate.NumberOfVolunteersRequired, _NewEvent.NumberOfVolunteersRequired);
            Assert.Equal(_EventToDuplicate.StartDateTime, _NewEvent.StartDateTime);
            Assert.Equal(_EventToDuplicate.EndDateTime, _NewEvent.EndDateTime);
            Assert.Equal(_EventToDuplicate.Location.Address1, _NewEvent.Location.Address1);
            Assert.Equal(_EventToDuplicate.Location.Address2, _NewEvent.Location.Address2);
            Assert.Equal(_EventToDuplicate.Location.City, _NewEvent.Location.City);
            Assert.Equal(_EventToDuplicate.Location.State, _NewEvent.Location.State);
            Assert.Equal(_EventToDuplicate.Location.PostalCode, _NewEvent.Location.PostalCode);
            Assert.Equal(_EventToDuplicate.Location.Name, _NewEvent.Location.Name);
            Assert.Equal(_EventToDuplicate.Location.PhoneNumber, _NewEvent.Location.PhoneNumber);
            Assert.Equal(_EventToDuplicate.Location.Country, _NewEvent.Location.Country);
            Assert.Equal(_EventToDuplicate.Tasks.Count(), _NewEvent.Tasks.Count());
            Assert.Equal(0, _NewEvent.UsersSignedUp.Count());
            Assert.Equal(_EventToDuplicate.Organizer.Id, _NewEvent.Organizer.Id);
            Assert.Equal(_EventToDuplicate.ImageUrl, _NewEvent.ImageUrl);
            Assert.Equal(_EventToDuplicate.RequiredSkills.Select(s => s.Skill.Id), _NewEvent.RequiredSkills.Select(s => s.Skill.Id));
            Assert.Equal(_EventToDuplicate.RequiredSkills.Count(), _NewEvent.RequiredSkills.Count());
            Assert.Equal(_EventToDuplicate.IsLimitVolunteers, _NewEvent.IsLimitVolunteers);
            Assert.Equal(_EventToDuplicate.IsAllowWaitList, _NewEvent.IsAllowWaitList);
        }

        [Fact]
        public void CreateANewLocationEntity()
        {
            Assert.False(_EventToDuplicate.Location.Id == _NewEvent.Location.Id);
        }

        [Fact]
        public void CreateNewTaskEntities()
        {
            // Todo: Improve this:
            // - Linqify
            // - Collection Equivalence
            // - Note: Equals and NotEquals assertions require expected values, 
            //         however in this case, we don't know. Well we do under test,
            //         but that would make the test brittle and dependant on the current test setup.

            for (int i = 0; i < _EventToDuplicate.Tasks.Count; i++)
            {
                Assert.False(_EventToDuplicate.Tasks[i].Id == _NewEvent.Tasks[i].Id);
            }
        }

        [Fact]
        public void MaintainOffsetBetweenTaskStartTimeAndEventStartTimeInNewTask()
        {
            var offsetsBetweenTaskStartTimeAndEventStartTimeForTaskToDuplicate =
                _EventToDuplicate.Tasks.Select(t => t.StartDateTime - _EventToDuplicate.StartDateTime);

            var offsetsBetweenTaskStartTimeAndEventStartTimeForNewTask =
                _NewEvent.Tasks.Select(t => t.StartDateTime - _NewEvent.StartDateTime);

            Assert.Equal(offsetsBetweenTaskStartTimeAndEventStartTimeForTaskToDuplicate,
                offsetsBetweenTaskStartTimeAndEventStartTimeForNewTask);
        }

        [Fact]
        public void MaintainTaskDurationInNewTask()
        {
            var eventToDuplicateTaskDurations = _EventToDuplicate.Tasks.Select(t => t.EndDateTime - t.StartDateTime);
            var newEventTaskDurations = _NewEvent.Tasks.Select(t => t.EndDateTime - t.StartDateTime);

            Assert.Equal(eventToDuplicateTaskDurations, newEventTaskDurations);
        }

        Event TestEvent()
        {
            return new Event()
            {
                //Id = 1,
                //CampaignId = 1,
                //Campaign = new Campaign()
                //{
                //    Id = 1
                //},
                //Name = "Name",
                //Description = "Description",
                //EventType = EventTypes.ItineraryManaged,
                //NumberOfVolunteersRequired = 10,
                //StartDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan()),
                //EndDateTime = new DateTimeOffset(2016, 1, 31, 0, 0, 0, new TimeSpan()),
                //Location = new Location()
                //{
                //    Id = 1,
                //    Address1 = "Address1",
                //    Address2 = "Address2",
                //    City = "City",
                //    State = "State",
                //    PostalCode = "PostalCode",
                //    Name = "Name",
                //    PhoneNumber = "PhoneNumber",
                //    Country = "Country"
                //},
                //Tasks = new List<AllReadyTask>()
                //{
                //    new AllReadyTask { Id = 1 },
                //    new AllReadyTask { Id = 2 },
                //    new AllReadyTask { Id = 3 }
                //},
                //UsersSignedUp = new List<EventSignup>()
                //{
                //    new EventSignup() { Id = 1 },
                //    new EventSignup() { Id = 2 },
                //    new EventSignup() { Id = 3 }
                //},
                //Organizer = new ApplicationUser() { Id = "Organiser" },
                //ImageUrl = "ImageUrl",
                //RequiredSkills = new List<EventSkill>()
                //{
                //    new EventSkill() { SkillId = 1, Skill = new Skill() { Id = 1 } },
                //    new EventSkill() { SkillId = 2, Skill = new Skill() { Id = 2 } },
                //    new EventSkill() { SkillId = 3, Skill = new Skill() { Id = 3 } }
                //},
                //IsLimitVolunteers = false,
                //IsAllowWaitList = true
            };
        }
    }
}
