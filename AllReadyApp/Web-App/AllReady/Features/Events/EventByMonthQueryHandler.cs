using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventByMonthQueryHandler : IRequestHandler<EventByDateRangeQuery, IEnumerable<EventViewModel>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public EventByMonthQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public IEnumerable<EventViewModel> Handle(EventByDateRangeQuery message)
        {
            var start = message.StartDate;
            var end = message.EndDate;

            return dataAccess.Events
                .Where(e => e.StartDateTime <= end && e.EndDateTime >= start)
                .Select(e => new EventViewModel(e));
        }
    }
}