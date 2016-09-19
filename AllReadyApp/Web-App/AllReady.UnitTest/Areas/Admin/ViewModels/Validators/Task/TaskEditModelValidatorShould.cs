using System;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Features.Events;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.ViewModels.Validators.Task
{
    public class TaskEditViewModelValidatorShould
    {
        private readonly DateTimeOffset eventStartDate = new DateTimeOffset(new DateTime(2016, 1, 1));
        private readonly DateTimeOffset eventEndDate = new DateTimeOffset(new DateTime(2020, 12, 31));

        [Fact]
        public async System.Threading.Tasks.Task ReturnCorrectErrorWhenEndDateTimeIsLessThanStartDateTime()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQueryAsync>())).ReturnsAsync(new Event { Id = 1, Campaign = new Campaign { TimeZoneId = "UTC" } });

            var validator = new TaskEditViewModelValidator(mockMediator.Object);

            var model = new EditViewModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            var errors = await validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be earlier than the start date");
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnsCorrectErrorWhenModelsStartDateTimeIsLessThanParentEventStartDate()
        {
            var validator = GetValidator();

            var model = new EditViewModel
            {
                StartDateTime = eventStartDate.AddDays(-10),
                EndDateTime = eventEndDate.AddDays(-1)
            };

            var errors = await validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "StartDateTime").Value, "Start date cannot be earlier than the event start date " + eventStartDate.ToString("d"));
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnsCorrectErrorWhenModelsEndDateTimeIsGreaterThanParentEventStartDate()
        {
            var validator = GetValidator();

            var model = new EditViewModel
            {
                StartDateTime = eventStartDate.AddDays(1),
                EndDateTime = eventEndDate.AddDays(10)
            };

            var errors = await validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "End date cannot be later than the event end date " + eventEndDate.ToString("d"));
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnsCorrectErrorWhenItineraryTaskWithStartAndEndDatesNotOnSameDay()
        {
            var validator = GetValidator();

            var model = new EditViewModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(1999, 12, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(2000, 12, 1))
            };

            var errors = await validator.Validate(model);

            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
            Assert.Equal(errors.Find(x => x.Key == "EndDateTime").Value, "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days");
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnsNoErrorForNonItineraryTaskWhenModelsDatesAreValid()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQueryAsync>())).ReturnsAsync(new Event
            {
                Id = 1,
                Campaign = new Campaign
                {
                    TimeZoneId = "UTC"
                },
                StartDateTime = eventStartDate,
                EndDateTime = eventEndDate,
                EventType = EventType.Rally
            });

            var validator = new TaskEditViewModelValidator(mockMediator.Object);

            var model = new EditViewModel
            {
                StartDateTime = eventStartDate.AddDays(1),
                EndDateTime = eventEndDate.AddDays(-1)
            };

            var errors = await validator.Validate(model);

            Assert.True(errors.Count == 0);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnsNoErrorForItineraryTaskWhenModelsDatesAreValid()
        {
            var validator = GetValidator();

            var model = new EditViewModel
            {
                StartDateTime = eventStartDate.AddDays(1),
                EndDateTime = eventStartDate.AddDays(1).AddHours(2)
            };

            var errors = await validator.Validate(model);

            Assert.True(errors.Count == 0);
        }

        private TaskEditViewModelValidator GetValidator()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQueryAsync>())).ReturnsAsync(new Event
            {
                Id = 1,
                Campaign = new Campaign
                {
                    TimeZoneId = "UTC"
                },
                StartDateTime = eventStartDate,
                EndDateTime = eventEndDate,
                EventType = EventType.Itinerary
            });

            return new TaskEditViewModelValidator(mockMediator.Object);
        }
    }
}