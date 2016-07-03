using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using Microsoft.EntityFrameworkCore;
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
            var eventId = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });
            var sut = await GetEvent(eventId);

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

            var eventId = await DuplicateEvent(duplicateEventModel);
            var sut = await GetEvent(eventId);

            Assert.Equal(1, sut.CampaignId);
            Assert.Equal(1, sut.Campaign.Id);
            Assert.Equal("Name", sut.Name);
            Assert.Equal("Description", sut.Description);
            Assert.Equal(EventType.Itinerary, sut.EventType);
            Assert.Equal(10, sut.NumberOfVolunteersRequired);
            Assert.Equal(new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan()), sut.StartDateTime);
            Assert.Equal(new DateTimeOffset(2016, 1, 31, 0, 0, 0, new TimeSpan()), sut.EndDateTime);
            Assert.Equal("Organizer", sut.Organizer.Id);
            Assert.Equal("ImageUrl", sut.ImageUrl);
            Assert.Equal(false, sut.IsLimitVolunteers);
            Assert.Equal(true, sut.IsAllowWaitList);
        }

        [Fact]
        public async Task CreateANewLocationEntity()
        {
            var eventId = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });
            var sut = await GetEvent(eventId);

            Assert.Equal(2, sut.Location.Id);
        }

        [Fact]
        public async Task CopyLocationPropertyValuesToTheNewLocation()
        {
            var eventId = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });
            var @event = await GetEvent(eventId);
            var sut = @event.Location;

            Assert.Equal("Address1", sut.Address1);
            Assert.Equal("Address2", sut.Address2);
            Assert.Equal("City", sut.City);
            Assert.Equal("State", sut.State);
            Assert.Equal("PostalCode", sut.PostalCode);
            Assert.Equal("Name", sut.Name);
            Assert.Equal("PhoneNumber", sut.PhoneNumber);
            Assert.Equal("Country", sut.Country);
        }


        [Fact]
        public async Task CreateNewTaskEntities()
        {
            var eventId = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });
            var @event = await GetEvent(eventId);
            var sut = @event.Tasks.OrderBy(t => t.Id).ToList();

            Assert.Equal(2, sut.Count());
            Assert.Equal(3, sut[0].Id);
            Assert.Equal(4, sut[1].Id);
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

            var eventId = await DuplicateEvent(duplicateEventModel);
            var @event = await GetEvent(eventId);
            var sut = @event.Tasks.OrderBy(t => t.StartDateTime).ToList();

            Assert.Equal(new DateTimeOffset(2016, 2, 1, 9, 0, 0, new TimeSpan()), sut[0].StartDateTime);
            Assert.Equal(new DateTimeOffset(2016, 2, 2, 10, 0, 0, new TimeSpan()), sut[1].StartDateTime);
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

            var eventId = await DuplicateEvent(duplicateEventModel);
            var @event = await GetEvent(eventId);
            var sut = @event.Tasks.OrderBy(t => t.StartDateTime).ToList();

            Assert.Equal(new TimeSpan(8, 0, 0), sut[0].EndDateTime - sut[0].StartDateTime);
            Assert.Equal(new TimeSpan(6, 0, 0), sut[1].EndDateTime - sut[1].StartDateTime);
        }

        [Fact]
        public async Task CreateNewTasksWithoutCopyingAssignedVolunteers()
        {
            var duplicateEventModel = new DuplicateEventModel()
            {
                Id = EVENT_TO_DUPLICATE_ID,
                Name = "Name",
                Description = "Description",
                StartDateTime = new DateTimeOffset(2016, 2, 1, 0, 0, 0, new TimeSpan()),
                EndDateTime = new DateTimeOffset(2016, 2, 28, 0, 0, 0, new TimeSpan())
            };

            var eventId = await DuplicateEvent(duplicateEventModel);
            var @event = await GetEvent(eventId);
            var sut = @event.Tasks.OrderBy(t => t.StartDateTime).ToList();

            Assert.Equal(0, sut[0].AssignedVolunteers.Count());
            Assert.Equal(0, sut[1].AssignedVolunteers.Count());
        }

        [Fact]
        public async Task CreateNewTasksWithTheSameRequiredSkills()
        {
            var eventId = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });
            var @event = await GetEvent(eventId);
            var sut = @event.Tasks.OrderBy(t => t.StartDateTime).ToList();

            Assert.Equal(2, sut[0].RequiredSkills.Count());
            Assert.Equal("Skill One", sut[0].RequiredSkills.OrderBy(ts => ts.Skill.Name).ToList()[0].Skill.Name);
            Assert.Equal("Skill Two", sut[0].RequiredSkills.OrderBy(ts => ts.Skill.Name).ToList()[1].Skill.Name);
            Assert.Equal(0, sut[1].RequiredSkills.Count());
        }

        [Fact]
        public async Task CreateNewEventWithoutCopyingUsersSignedUp()
        {
            var eventId = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });
            var sut = await GetEvent(eventId);

            Assert.Equal(0, sut.UsersSignedUp.Count());
        }

        [Fact]
        public async Task CreateNewEventWithTheSameRequiredSkills()
        {
            var eventId = await DuplicateEvent(new DuplicateEventModel() { Id = EVENT_TO_DUPLICATE_ID });
            var @event = await GetEvent(eventId);
            var sut = @event.RequiredSkills.OrderBy(es => es.Skill.Name).ToList();

            Assert.Equal(2, sut.Count());
            Assert.Equal("Skill One", sut[0].Skill.Name);
            Assert.Equal("Skill Two", sut[1].Skill.Name);
        }

        #region Helpers
        const int EVENT_TO_DUPLICATE_ID = 1;

        async Task<Event> GetEvent(int eventId)
        {
            return await Context.Events
                .AsNoTracking()
                .Include(e => e.Campaign)
                .Include(e => e.Location)
                .Include(e => e.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .Include(e => e.Organizer)
                .Include(e => e.RequiredSkills).ThenInclude(es => es.Skill)
                .SingleAsync(e => e.Id == eventId);
        }

        async Task<int> DuplicateEvent(DuplicateEventModel duplicateEventModel)
        {
            var command = new DuplicateEventCommand() { DuplicateEventModel = duplicateEventModel };
            var handler = new DuplicateEventCommandHandler(Context);
            return await handler.Handle(command);
        }

        protected override void LoadTestData()
        {
            var skillOne = new Skill() { Name = "Skill One" };
            var skillTwo = new Skill() { Name = "Skill Two" };

            Context.AddRange(skillOne, skillTwo);


            var @event = new Event()
            {
                Campaign = new Campaign(),
                Name = "Name",
                Description = "Description",
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 10,
                StartDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan()),
                EndDateTime = new DateTimeOffset(2016, 1, 31, 0, 0, 0, new TimeSpan()),
                Location = new Location()
                {
                    Address1 = "Address1",
                    Address2 = "Address2",
                    City = "City",
                    State = "State",
                    PostalCode = "PostalCode",
                    Name = "Name",
                    PhoneNumber = "PhoneNumber",
                    Country = "Country"
                },
                Tasks = new List<AllReadyTask>()
                {
                    new AllReadyTask()
                    {
                        StartDateTime = new DateTimeOffset(2016, 1, 1, 9, 0, 0, new TimeSpan()),
                        EndDateTime = new DateTimeOffset(2016, 1, 1, 17, 0, 0, new TimeSpan()),
                        AssignedVolunteers = new List<TaskSignup>()
                        {
                            new TaskSignup(),
                            new TaskSignup()
                        },
                        RequiredSkills = new List<TaskSkill>()
                        {
                            new TaskSkill() { Skill = skillOne },
                            new TaskSkill() { Skill = skillTwo },
                        },
                    },
                    new AllReadyTask()
                    {
                        StartDateTime = new DateTimeOffset(2016, 1, 2, 10, 0, 0, new TimeSpan()),
                        EndDateTime = new DateTimeOffset(2016, 1, 2, 16, 0, 0, new TimeSpan()),
                        AssignedVolunteers = new List<TaskSignup>()
                        {
                            new TaskSignup(),
                            new TaskSignup()
                        }
                    },
                },
                UsersSignedUp = new List<EventSignup>()
                {
                    new EventSignup(),
                    new EventSignup(),
                },
                Organizer = new ApplicationUser() { Id = "Organizer" },
                ImageUrl = "ImageUrl",
                RequiredSkills = new List<EventSkill>()
                {
                    new EventSkill() { Skill = skillOne },
                    new EventSkill() { Skill = skillTwo },
                },
                IsLimitVolunteers = false,
                IsAllowWaitList = true
            };

            Context.Add(@event.Campaign);
            Context.Add(@event.Location);
            Context.Add(@event.Organizer);
            Context.Add(@event);
            Context.SaveChanges();

        }
        #endregion
    }
}
