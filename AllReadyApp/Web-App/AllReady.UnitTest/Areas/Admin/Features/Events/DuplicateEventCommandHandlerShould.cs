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
            Assert.Equal(2, sut.Tasks.Count());
            Assert.Equal(0, sut.UsersSignedUp.Count());
            Assert.Equal("Organizer", sut.Organizer.Id);
            Assert.Equal("ImageUrl", sut.ImageUrl);
            // Todo: Implement Copying Required Skills
            //Assert.Equal(_EventToDuplicate.RequiredSkills.Select(s => s.Skill.Id), sut.RequiredSkills.Select(s => s.Skill.Id));
            //Assert.Equal(3, sut.RequiredSkills.Count());
            Assert.Equal(false, sut.IsLimitVolunteers);
            Assert.Equal(true, sut.IsAllowWaitList);
        }

        [Fact]
        public async Task CreateANewLocationEntity()
        {
            var sut = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });

            Assert.Equal(2, sut.Location.Id);
        }

        [Fact]
        public async Task CreateNewTaskEntities()
        {
            var sut = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });

            Assert.Equal(2, sut.Tasks.Count());
            Assert.Equal(3, sut.Tasks[0].Id);
            Assert.Equal(4, sut.Tasks[1].Id);
        }

        [Fact]
        public async Task MaintainOffsetBetweenTaskStartTimeAndEventStartTimeInNewTask()
        {
            var duplicateEventModel = new DuplicateEventModel()
            {
                Id = EVENT_TO_DUPLICATE_ID,
                Name = "Name",
                Description = "Description",
                StartDateTime = new DateTimeOffset(2016, 2, 1, 0, 0, 0, new TimeSpan()),
                EndDateTime = new DateTimeOffset(2016, 2, 28, 0, 0, 0, new TimeSpan())
            };

            var sut = await DuplicateEvent(duplicateEventModel);

            Assert.Equal(new DateTimeOffset(2016, 2, 1, 9, 0, 0, new TimeSpan()), sut.Tasks[0].StartDateTime);
            Assert.Equal(new DateTimeOffset(2016, 2, 2, 10, 0, 0, new TimeSpan()), sut.Tasks[1].StartDateTime);
        }

        [Fact]
        public async Task MaintainTaskDurationInNewTask()
        {
            var duplicateEventModel = new DuplicateEventModel()
            {
                Id = EVENT_TO_DUPLICATE_ID,
                Name = "Name",
                Description = "Description",
                StartDateTime = new DateTimeOffset(2016, 2, 1, 0, 0, 0, new TimeSpan()),
                EndDateTime = new DateTimeOffset(2016, 2, 28, 0, 0, 0, new TimeSpan())
            };

            var sut = await DuplicateEvent(duplicateEventModel);

            Assert.Equal(new TimeSpan(8, 0, 0), sut.Tasks[0].EndDateTime - sut.Tasks[0].StartDateTime);
            Assert.Equal(new TimeSpan(6, 0, 0), sut.Tasks[1].EndDateTime - sut.Tasks[1].StartDateTime);
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

            var location = new Location()
            {
                Address1 = "Address1",
                Address2 = "Address2",
                City = "City",
                State = "State",
                PostalCode = "PostalCode",
                Name = "Name",
                PhoneNumber = "PhoneNumber",
                Country = "Country"
            };

            var tasks = new List<AllReadyTask>()
                {
                    new AllReadyTask()
                    {
                        StartDateTime = new DateTimeOffset(2016, 1, 1, 9, 0, 0, new TimeSpan()),
                        EndDateTime = new DateTimeOffset(2016, 1, 1, 17, 0, 0, new TimeSpan())
                    },
                    new AllReadyTask()
                    {
                        StartDateTime = new DateTimeOffset(2016, 1, 2, 10, 0, 0, new TimeSpan()),
                        EndDateTime = new DateTimeOffset(2016, 1, 2, 16, 0, 0, new TimeSpan())
                    },
                };

            var organiser = new ApplicationUser() { Id = "Organizer" };

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
                Location = location,
                Tasks = tasks,
                UsersSignedUp = new List<EventSignup>()
                {
                    new EventSignup() { Id = 1 },
                    new EventSignup() { Id = 2 },
                    new EventSignup() { Id = 3 }
                },
                Organizer = organiser,
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
            Context.Add(location);
            Context.AddRange(tasks);
            Context.Add(organiser);
            Context.Add(@event);
            Context.SaveChanges();

        }
        #endregion
    }
}
