using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivititesByGeographyQueryHandler : IRequestHandler<ActivitiesByGeographyQuery, List<Models.Activity>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ActivititesByGeographyQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<Models.Activity> Handle(ActivitiesByGeographyQuery message)
        {
            return dataAccess.ActivitiesByGeography(message.Latitude, message.Longitude, message.Miles).ToList();
        }
    }
}
