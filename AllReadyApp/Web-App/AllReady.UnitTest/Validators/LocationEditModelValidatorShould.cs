using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Validators
{
    public class LocationEditModelValidatorShould
    {
        [Fact]
        public async Task ReportErrorsWhenPostalCodeInvalid()
        {
            // arrage
            var mediator = new Mock<IMediator>();
            var validator = new LocationEditModelValidator(mediator.Object);
            var model = new LocationEditModel()
            {
                PostalCode = "12345",
                State = "WA",
                City = "Seattle"
            };

            // act
            var errors = await validator.Validate(model);

            // assert
            Assert.True(errors.ContainsKey("PostalCode"));
        }
    }
}
