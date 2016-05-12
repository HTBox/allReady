using AllReady.Areas.Admin.Features.Shared;
using AllReady.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models.Validators
{
    public class LocationEditModelValidator
    {
        private IMediator _mediator;

        public LocationEditModelValidator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Dictionary<string, string> Validate(LocationEditModel model)
        {
            var result = new Dictionary<string, string>();

            return result;
        }
    }
}
