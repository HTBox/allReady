using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Models.Validators
{
    public class ActivityDetailModelValidatorShould
    {
        [Fact]
        public async Task ReportErrorsWhenEndDateIsInvalid()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new ActivityDetailModelValidator(mediator.Object);
            var campaign = new CampaignSummaryModel();
            var model = new ActivityDetailModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDateTime = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            // act
            var errors = await validator.Validate(model, campaign);

            // assert
            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
        }

        [Fact]
        public async Task ReportErrorsWhenEndDateOutsideCampaignWindow()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new ActivityDetailModelValidator(mediator.Object);
            var campaign = new CampaignSummaryModel()
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            var model = new ActivityDetailModel
            {
                EndDateTime = new DateTimeOffset(new DateTime(2001, 1, 1))
            };

            // act
            var errors = await validator.Validate(model, campaign);

            // assert
            Assert.True(errors.Exists(x => x.Key.Equals("EndDateTime")));
        }

        [Fact]
        public async Task ReportErrorsWhenStartDateOutsideCampaignWindow()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new ActivityDetailModelValidator(mediator.Object);
            var campaign = new CampaignSummaryModel()
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2001, 1, 1))
            };
            var model = new ActivityDetailModel
            {
                StartDateTime = new DateTimeOffset(new DateTime(1999, 1, 1)),
            };

            // act
            var errors = await validator.Validate(model, campaign);

            // assert
            Assert.True(errors.Exists(x => x.Key.Equals("StartDateTime")));
        }

        [Fact]
        public async Task ReportErrorsWhenPostalCodeInvalid()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new ActivityDetailModelValidator(mediator.Object);
            var campaign = new CampaignSummaryModel();
            var model = new ActivityDetailModel
            {
                Location = new LocationEditModel()
                {
                    PostalCode = "12345",
                    State = "WA",
                    City = "Seattle"
                }
            };

            // act
            var errors = await validator.Validate(model, campaign);

            // assert
            Assert.True(errors.Exists(x => x.Key.Equals("PostalCode")));
        }
    }
}
