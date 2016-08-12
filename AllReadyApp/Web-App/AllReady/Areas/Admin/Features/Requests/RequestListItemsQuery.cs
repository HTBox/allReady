using MediatR;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Request;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestListItemsQuery : IAsyncRequest<List<RequestListModel>>
    {
        public RequestSearchCriteria Criteria { get; set; }
    }
}
