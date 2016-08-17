using AllReady.Features.Event;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Areas.Admin.ViewModels.Validators;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Models.Validators
{
    public class TaskSummaryModelValidatorTests
    {
        private readonly DateTimeOffset eventStartDate = new DateTimeOffset(new DateTime(2016, 1, 1));
        private readonly DateTimeOffset eventEndDate = new DateTimeOffset(new DateTime(2020, 12, 31));

        [Fact]
        public void ReturnsCorrectErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Id = 1, Campaign = new Campaign { TimeZoneId = "UTC" } });

            var validator = new TaskEditViewModelValidator(mockMediator.Object);

            var model = new TaskSummaryViewModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            var errors = validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be earlier than the start date");
        }

        [Fact]
        public void ReturnsCorrectErrorWhenModelsStartDateTimeIsLessThanParentEventStartDate()
        {
            var validator = GetValidator();

            var model = new TaskSummaryViewModel
            {
                StartDateTime = eventStartDate.AddDays(-10),
                EndDateTime = eventEndDate.AddDays(-1)
            };

            var errors = validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "StartDateTime").Value, "Start date cannot be earlier than the event start date " + eventStartDate.ToString("d"));
        }

        [Fact]
        public void ReturnsCorrectErrorWhenModelsEndDateTimeIsGreaterThanParentEventStartDate()
        {
            var validator = GetValidator();

            var model = new TaskSummaryViewModel
            {
                StartDateTime = eventStartDate.AddDays(1),
                EndDateTime = eventEndDate.AddDays(10)
            };

            var errors = validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be later than the event end date " + eventEndDate.ToString("d"));
        }

        [Fact]
        public void ReturnsCorrectErrorWhenItineraryTaskWithStartAndEndDatesNotOnSameDay()
        {
            var validator = GetValidator();

            var model = new TaskSummaryViewModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(1999, 12, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(2000, 12, 1))
            };

            var errors = validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days");
        }

        [Fact]
        public void ReturnsNoErrorForNonItineraryTaskWhenModelsDatesAreValid()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Id = 1,
                Campaign = new Campaign
                {
                    TimeZoneId = "UTC",
                },
                StartDateTime = eventStartDate,
                EndDateTime = eventEndDate,
                EventType = EventType.Rally
            });

            var validator = new TaskEditViewModelValidator(mockMediator.Object);

            var model = new TaskSummaryViewModel
            {
                StartDateTime = eventStartDate.AddDays(1),
                EndDateTime = eventEndDate.AddDays(-1)
            };

            var errors = validator.Validate(model);

            Assert.True(errors.Count == 0);
        }

        [Fact]
        public void ReturnsNoErrorForItineraryTaskWhenModelsDatesAreValid()
        {
            var validator = GetValidator();

            var model = new TaskSummaryViewModel
            {
                StartDateTime = eventStartDate.AddDays(1),
                EndDateTime = eventStartDate.AddDays(1).AddHours(2),
            };

            var errors = validator.Validate(model);

            Assert.True(errors.Count == 0);
        }

        private TaskEditViewModelValidator GetValidator()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event
            {
                Id = 1,
                Campaign = new Campaign
                {
                    TimeZoneId = "UTC",
                },
                StartDateTime = eventStartDate,
                EndDateTime = eventEndDate,
                EventType = EventType.Itinerary
            });

            return new TaskEditViewModelValidator(mockMediator.Object);
        }
    }
}
