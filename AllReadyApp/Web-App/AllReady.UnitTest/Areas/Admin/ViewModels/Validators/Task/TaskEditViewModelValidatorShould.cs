using System;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Areas.Admin.ViewModels.Validators.Task;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.Providers;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.ViewModels.Validators.Task
{
    public class TaskEditViewModelValidatorShould
    {
        [Fact]
        public async System.Threading.Tasks.Task SendEventByIdQueryWithCorrectEventId()
        {
            var model = new EditViewModel { EventId = 1 };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(new Event { Campaign = new Campaign() });

            var sut = new TaskEditViewModelValidator(mediator.Object, Mock.Of<IConvertDateTimeOffset>());
            await sut.Validate(model);

            mediator.Verify(x => x.SendAsync(It.Is<EventByEventIdQuery>(y => y.EventId == model.EventId)), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task InvokeGetDateTimeOffsetWithCorrectParametersForStartDate()
        {
            var now = DateTimeOffset.Now;

            var model = new EditViewModel { EventId = 1, StartDateTime = now, EndDateTime = now };
            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC" }};

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();

            var sut = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            await sut.Validate(model);

            dateTimeOffsetProvider.Verify(x => x.ConvertDateTimeOffsetTo(@event.Campaign.TimeZoneId, model.StartDateTime, model.StartDateTime.Hour,
                model.StartDateTime.Minute, 0));
        }

        [Fact]
        public async System.Threading.Tasks.Task InvokeGetDateTimeOffsetWithCorrectParametersForEndDate()
        {
            var now = DateTimeOffset.Now;

            var model = new EditViewModel { EventId = 1, StartDateTime = now, EndDateTime = now };
            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC" } };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();

            var sut = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            await sut.Validate(model);

            dateTimeOffsetProvider.Verify(x => x.ConvertDateTimeOffsetTo(@event.Campaign.TimeZoneId, model.EndDateTime, model.EndDateTime.Hour,
                model.EndDateTime.Minute, 0));
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC" }};

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();
            dateTimeOffsetProvider.SetupSequence(x => x.ConvertDateTimeOffsetTo(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now.AddDays(1))
                .Returns(now.AddDays(-1));

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = await validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be earlier than the start date");
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenModelsStartDateTimeIsLessThanParentEventStartDate()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now.AddDays(1), EndDateTime = now.AddDays(-1) };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();
            dateTimeOffsetProvider.Setup(x => x.ConvertDateTimeOffsetTo(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now);

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = await validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "StartDateTime").Value, "Start date cannot be earlier than the event start date " + @event.StartDateTime.ToString("d"));
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenModelsEndDateTimeIsGreaterThanParentEventStartDate()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(-1) };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();
            dateTimeOffsetProvider.Setup(x => x.ConvertDateTimeOffsetTo(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now);

            var validator = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            var errors = await validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be later than the event end date " + @event.EndDateTime.ToString("d"));
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenItineraryTaskWithStartAndEndDatesNotOnSameDay()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Itinerary };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();
            dateTimeOffsetProvider.SetupSequence(x => x.ConvertDateTimeOffsetTo(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now)
                .Returns(now.AddDays(1));

            var validator = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            var errors = await validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days");
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnNoErrorForNonItineraryTaskWhenModelsDatesAreValid()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Rally };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();
            dateTimeOffsetProvider.SetupSequence(x => x.ConvertDateTimeOffsetTo(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now)
                .Returns(now.AddDays(1));

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = await validator.Validate(new EditViewModel());

            Assert.True(errors.Count == 0);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnNoErrorForItineraryTaskWhenModelsDatesAreValid()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Itinerary };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var dateTimeOffsetProvider = new Mock<IConvertDateTimeOffset>();
            dateTimeOffsetProvider.SetupSequence(x => x.ConvertDateTimeOffsetTo(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now)
                .Returns(now);

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = await validator.Validate(new EditViewModel());

            Assert.True(errors.Count == 0);
        }
    }
}