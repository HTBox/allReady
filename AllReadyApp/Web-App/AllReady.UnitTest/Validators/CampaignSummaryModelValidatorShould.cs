using AllReady.Areas.Admin.Features.Shared;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Validators
{
    public class CampaignSummaryModelValidatorShould
    {
        [Fact]
        public async Task ReportErrorsWhenDatesAreInvalid()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new CampaignSummaryModelValidator(mediator.Object);
            var model = new CampaignSummaryModel
            {
                StartDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(1999, 1, 1))
            };

            // act
            var errors = await validator.Validate(model);

            // assert
            Assert.True(errors.ContainsKey("EndDate"));
        }

        [Fact]
        public async Task ReportErrorsWhenPostalCodeIsInvalid()
        {
            // arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<CheckValidPostcodeQueryAsync>())).ReturnsAsync(false);

            var validator = new CampaignSummaryModelValidator(mediator.Object);
            var model = new CampaignSummaryModel
            {
                Location = new LocationEditModel { PostalCode = "90210" }
            };

            // act
            var errors = await validator.Validate(model);

            // assert
            mediator.Verify(m => m.SendAsync(It.IsAny<CheckValidPostcodeQueryAsync>()));
            Assert.True(errors.ContainsKey("PostalCode"));

        }

    }
}
