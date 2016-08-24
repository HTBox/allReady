using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Event;
using AllReady.Models;
using AllReady.ViewModels.Event;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventsByDateRangeQueryHandlerTests
    {
        [Fact]
        public void CallsEventsPropertyOnce()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());

            var message = new EventByDateRangeQuery { StartDate = may, EndDate = june };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new EventByDateRangeQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.Events, Times.Once());
        }

        [Fact]
        public void FiltersEventsCorrectly()
        {
            var may = new DateTimeOffset(2016, 5, 1, 0, 0, 0, new TimeSpan());
            var june = new DateTimeOffset(2016, 6, 1, 0, 0, 0, new TimeSpan());

            var message = new EventByDateRangeQuery { StartDate = may, EndDate = june };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var inRange = new Models.Event {Id = 2, StartDateTime = may, EndDateTime = june};
            var startBeforeRangeEndsInRange = new Models.Event {Id = 1, StartDateTime = may.AddMonths(-1), EndDateTime = june};
            var startInRangeEndsAfterRange = new Models.Event {Id = 3, StartDateTime = may, EndDateTime = june.AddMonths(1)};
            var startsAndEndsAfterRange = new Models.Event {Id = 4, StartDateTime = may.AddMonths(2), EndDateTime = june.AddMonths(2)};
            var startsAndEndsBeforRange = new Models.Event {Id = 0, StartDateTime = may.AddMonths(-2), EndDateTime = june.AddMonths(-2)};

            var events = new List<Models.Event>
            {
                startsAndEndsBeforRange,
                startBeforeRangeEndsInRange,
                inRange,
                startInRangeEndsAfterRange,
                startsAndEndsAfterRange,
            };

            dataAccess.Setup(x => x.Events).Returns(events);

            var sut = new EventByDateRangeQueryHandler(dataAccess.Object);
            var result = sut.Handle(message).ToArray();

            Assert.Equal(3, result.Length);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
            Assert.Equal(3, result[2].Id);
        }

    }
}
