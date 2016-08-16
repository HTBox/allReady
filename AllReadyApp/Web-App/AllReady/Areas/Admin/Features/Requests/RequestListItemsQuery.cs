using MediatR;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Areas.Admin.ViewModels.Itinerary;

namespace AllReady.Areas.Admin.Features.Requests
{

    public class RequestListItemsQuery : IAsyncRequest<List<RequestListViewModel>>
    {
        public RequestSearchCriteria Criteria { get; set; }
    }
}
