using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Areas.Admin.ViewModels.Validators.Task;
using AllReady.Features.Event;
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
        public void SendEventByIdQueryWithCorrectEventId()
        {
            var model = new EditViewModel { EventId = 1 };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign() });

            var sut = new TaskEditViewModelValidator(mediator.Object, Mock.Of<IDateTimeOffsetProvider>());
            sut.Validate(model);

            mediator.Verify(x => x.Send(It.Is<EventByIdQuery>(y => y.EventId == model.EventId)), Times.Once);
        }

        [Fact]
        public void InvokeGetDateTimeOffsetWithCorrectParametersForStartDate()
        {
            var now = DateTimeOffset.Now;

            var model = new EditViewModel { EventId = 1, StartDateTime = now, EndDateTime = now };
            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC" }};

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();

            var sut = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            sut.Validate(model);

            dateTimeOffsetProvider.Verify(x => x.GetDateTimeOffsetFor(@event.Campaign.TimeZoneId, model.StartDateTime, model.StartDateTime.Hour,
                model.StartDateTime.Minute, 0));
        }

        [Fact]
        public void InvokeGetDateTimeOffsetWithCorrectParametersForEndDate()
        {
            var now = DateTimeOffset.Now;

            var model = new EditViewModel { EventId = 1, StartDateTime = now, EndDateTime = now };
            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC" } };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();

            var sut = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            sut.Validate(model);

            dateTimeOffsetProvider.Verify(x => x.GetDateTimeOffsetFor(@event.Campaign.TimeZoneId, model.EndDateTime, model.EndDateTime.Hour,
                model.EndDateTime.Minute, 0));
        }

        [Fact]
        public void ReturnCorrectErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC" }};

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            dateTimeOffsetProvider.SetupSequence(x => x.GetDateTimeOffsetFor(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now.AddDays(1))
                .Returns(now.AddDays(-1));

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be earlier than the start date");
        }

        [Fact]
        public void ReturnCorrectErrorWhenModelsStartDateTimeIsLessThanParentEventStartDate()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now.AddDays(1), EndDateTime = now.AddDays(-1) };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            dateTimeOffsetProvider.Setup(x => x.GetDateTimeOffsetFor(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now);

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "StartDateTime").Value, "Start date cannot be earlier than the event start date " + @event.StartDateTime.ToString("d"));
        }

        [Fact]
        public void ReturnCorrectErrorWhenModelsEndDateTimeIsGreaterThanParentEventStartDate()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(-1) };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            dateTimeOffsetProvider.Setup(x => x.GetDateTimeOffsetFor(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now);

            var validator = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            var errors = validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be later than the event end date " + @event.EndDateTime.ToString("d"));
        }

        [Fact]
        public void ReturnCorrectErrorWhenItineraryTaskWithStartAndEndDatesNotOnSameDay()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Itinerary };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            dateTimeOffsetProvider.SetupSequence(x => x.GetDateTimeOffsetFor(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now)
                .Returns(now.AddDays(1));

            var validator = new TaskEditViewModelValidator(mediator.Object, dateTimeOffsetProvider.Object);
            var errors = validator.Validate(new EditViewModel());

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days");
        }

        [Fact]
        public void ReturnNoErrorForNonItineraryTaskWhenModelsDatesAreValid()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Rally };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            dateTimeOffsetProvider.SetupSequence(x => x.GetDateTimeOffsetFor(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now)
                .Returns(now.AddDays(1));

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = validator.Validate(new EditViewModel());

            Assert.True(errors.Count == 0);
        }

        [Fact]
        public void ReturnNoErrorForItineraryTaskWhenModelsDatesAreValid()
        {
            var now = DateTimeOffset.Now;

            var @event = new Event { Campaign = new Campaign { TimeZoneId = "UTC", }, StartDateTime = now, EndDateTime = now.AddDays(1), EventType = EventType.Itinerary };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(@event);

            var dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            dateTimeOffsetProvider.SetupSequence(x => x.GetDateTimeOffsetFor(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(now)
                .Returns(now);

            var validator = new TaskEditViewModelValidator(mockMediator.Object, dateTimeOffsetProvider.Object);
            var errors = validator.Validate(new EditViewModel());

            Assert.True(errors.Count == 0);
        }
    }
}