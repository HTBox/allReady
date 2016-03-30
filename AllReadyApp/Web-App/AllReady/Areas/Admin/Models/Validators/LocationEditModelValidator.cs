using AllReady.Areas.Admin.Features.Shared;
using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Dictionary<string, string>> Validate(LocationEditModel model)
        {
            var result = new Dictionary<string, string>();

            // Temporary code to avoid current database update error when the post code geo does not exist in the database.
            if (!string.IsNullOrEmpty(model?.PostalCode))
            {
                bool validPostcode = await _mediator.SendAsync(new CheckValidPostcodeQueryAsync
                {
                    Postcode = new PostalCodeGeo
                    {
                        City = model.City,
                        State = model.State,
                        PostalCode = model.PostalCode
                    }
                });

                if (!validPostcode)
                {
                    result.Add(nameof(model.PostalCode), "The city, state and postal code combination is not valid");
                }
            }
            return result;
        }
    }
}
