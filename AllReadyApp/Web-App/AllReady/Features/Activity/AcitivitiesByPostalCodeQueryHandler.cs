using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class AcitivitiesByPostalCodeQueryHandler : IRequestHandler<AcitivitiesByPostalCodeQuery, List<Models.Activity>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public AcitivitiesByPostalCodeQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<Models.Activity> Handle(AcitivitiesByPostalCodeQuery message)
        {
            return dataAccess.ActivitiesByPostalCode(message.PostalCode, message.Distance).ToList();
        }
    }
}
