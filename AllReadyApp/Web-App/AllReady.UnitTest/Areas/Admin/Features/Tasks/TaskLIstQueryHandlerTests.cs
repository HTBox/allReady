using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class TaskListQueryHandlerTests : InMemoryContextTest
    {
        private AllReadyTask _task;
        private AllReadyContext context;
        private Random rnd;
        private AllReadyTask GenerateTask()
        {
            return new AllReadyTask
            {
                Id = rnd.Next(3, 1000),
                Name = "TestTask",
                StartDateTime = new DateTimeOffset(DateTime.Today),
                EndDateTime = new DateTimeOffset(DateTime.Today.AddDays(1)),
                NumberOfVolunteersRequired = 2
            };
        }

        protected override void LoadTestData()
        {
            rnd = new Random();
            context = ServiceProvider.GetService<AllReadyContext>();
            var tasks = context.Tasks.ToList();
            _task = GenerateTask();

            var htb = new Organization
            {
                Name = "Humanitarian Test",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Name = "test Campaign",
                ManagingOrganization = htb
            };

            var myEvent = new Event
            {
                Id = rnd.Next(3,1000),
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>()
            };

            _task.Event = myEvent;
            context.Organizations.Add(htb);

            context.Campaigns.Add(firePrev);
            context.Events.Add(myEvent);
            context.Tasks.Add(_task);
            tasks = context.Tasks.ToList();
            context.SaveChanges();
        }

        [Fact]
        public void TaskListQueryMade()
        {
            var handler = new TaskListQueryHandler(context);
            var taskListQuery = new TaskListQuery
            {
                EventId = _task.Event.Id
            };
            var result = handler.Handle(taskListQuery).ToList();
            Assert.NotNull(result);

            Assert.Equal(result.Count, 1);

            var task = result.First();
            Assert.Equal(task.Id, _task.Id);
            Assert.Equal(task.Name, _task.Name);
            Assert.Equal(task.StartDateTime, _task.StartDateTime);
            Assert.Equal(task.EndDateTime, _task.EndDateTime);
            Assert.Equal(task.NumberOfVolunteersRequired, _task.NumberOfVolunteersRequired);
            Assert.False(task.IsUserSignedUpForTask);
        }
    }
}
