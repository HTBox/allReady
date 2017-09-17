using System;

using AllReady.Areas.Admin.ViewModels.Validators.VolunteerTask;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Features.Events;
using AllReady.Models;

using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.ViewModels.Validators.VolunteerTask
{
    public class VolunteerTaskEditViewModelValidatorShould
    {
        [Fact]
        public async System.Threading.Tasks.Task SendEventByIdQueryWithCorrectEventId()
        {
            var model = new EditViewModel { EventId = 1 };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event { Campaign = new Campaign() });

            var sut = new VolunteerTaskEditViewModelValidator(mediator.Object);
            await sut.Validate(model);

            mediator.Verify(x => x.SendAsync(It.Is<EventByEventIdQuery>(y => y.EventId == model.EventId)), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event {
                Campaign = new Campaign { TimeZoneId = "UTC" },
                TimeZoneId = "UTC"};
            
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var validator = new VolunteerTaskEditViewModelValidator(mockMediator.Object);
            var errors = await validator.Validate(new EditViewModel
            {
                StartDateTime = now,
                EndDateTime = now.AddMinutes(-30)
            });

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal("End date cannot be earlier than the start date", errors.Find(x => x.Key == "EndDateTime").Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenModelsStartDateTimeIsLessThanParentEventStartDate()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, TimeZoneId = "UTC", StartDateTime = now.AddDays(1), EndDateTime = now.AddDays(-1) };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);
            
            var validator = new VolunteerTaskEditViewModelValidator(mockMediator.Object);
            var errors = await validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "StartDateTime").Value, String.Format("Start date cannot be earlier than the event start date {0:g}.", @event.StartDateTime));
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenModelsEndDateTimeIsGreaterThanParentEventStartDate()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", },
                StartDateTime = now,
                EndDateTime = now.AddDays(-1) };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var validator = new VolunteerTaskEditViewModelValidator(mediator.Object);
            var errors = await validator.Validate(new EditViewModel
            {
                EndDateTime = now.AddDays(-1).AddMinutes(15)
            });

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, String.Format("The end date of this task cannot be after the end date of the event {0:g}", @event.EndDateTime));
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenItineraryTaskWithStartAndEndDatesNotOnSameDay()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Itinerary };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var validator = new VolunteerTaskEditViewModelValidator(mediator.Object);
            var errors = await validator.Validate(new EditViewModel
            {
                StartDateTime = now.AddDays(-1),
                EndDateTime = now
            });

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal("For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days", errors.Find(x => x.Key == "EndDateTime").Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnNoErrorForNonItineraryTaskWhenModelsDatesAreValid()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC" }, TimeZoneId = "UTC", StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Rally };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            
            var validator = new VolunteerTaskEditViewModelValidator(mockMediator.Object);
            var errors = await validator.Validate(new EditViewModel
            {
                StartDateTime = now.AddMinutes(30),
                EndDateTime = now.AddDays(1).AddMinutes(-30)
            });

            Assert.True(errors.Count == 0);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnNoErrorForItineraryTaskWhenModelsDatesAreValid()
        {
            var now = new DateTimeOffset(2016, 10, 4, 8, 0, 0, TimeSpan.FromSeconds(0));

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), TimeZoneId = "UTC", EventType = EventType.Itinerary };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var validator = new VolunteerTaskEditViewModelValidator(mockMediator.Object);
            var errors = await validator.Validate(new EditViewModel
            {
                StartDateTime = now.AddMinutes(15),
                EndDateTime = now.AddMinutes(120)
            });

            Assert.True(errors.Count == 0);
        }
    }
}