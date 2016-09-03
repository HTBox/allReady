using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Events;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventsByDateRangeQueryHandlerTests : InMemoryContextTest
    {

        [Fact]
        public void CallsEventsPropertyOnce()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());

            var sut = new EventByDateRangeQueryHandler(Context);
            var result = sut.Handle(new EventByDateRangeQuery { StartDate = may, EndDate = june });

            Assert.NotNull(result);
        }

        [Fact]
        public void FiltersEventsCorrectly()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());

            var events = GetEvents(may, june);

            Context.Events.AddRange(events);
            Context.SaveChanges();

            var sut = new EventByDateRangeQueryHandler(Context);
            var result = sut.Handle(new EventByDateRangeQuery { StartDate = may, EndDate = june }).ToArray();

            Context.Events.RemoveRange(events);
            Context.SaveChanges();

            Assert.Equal(3, result.Length);

            Assert.Equal(3, result[0].Id);
            Assert.Equal(4, result[1].Id);
            Assert.Equal(5, result[2].Id);
        }

        private static List<Models.Event> GetEvents(DateTimeOffset may, DateTimeOffset june)
        {
            var inRange = new Models.Event { StartDateTime = may, EndDateTime = june };
            var startBeforeRangeEndsInRange = new Models.Event { StartDateTime = may.AddMonths(-1), EndDateTime = june };
            var startInRangeEndsAfterRange = new Models.Event { StartDateTime = may, EndDateTime = june.AddMonths(1) };
            var startsAndEndsAfterRange = new Models.Event { StartDateTime = may.AddMonths(2), EndDateTime = june.AddMonths(2) };
            var startsAndEndsBeforRange = new Models.Event { StartDateTime = may.AddMonths(-2), EndDateTime = june.AddMonths(-2) };

            var events = new List<Models.Event>
            {
                startsAndEndsBeforRange,
                startBeforeRangeEndsInRange,
                inRange,
                startInRangeEndsAfterRange,
                startsAndEndsAfterRange,
            };
            return events;
        }

        protected override void LoadTestData()
        {
            var ev = new Models.Event { Name = "Some Event" };

            var dbSet = Context.Events;
            dbSet.Add(ev);


            Context.SaveChanges();
        }
    }
}
