using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Areas.Admin.Models.RequestModels;
using MediatR;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestListItemsQuery : IAsyncRequest<List<RequestListModel>>
    {
        public RequestSearchCriteria Criteria { get; set; }
    }
}
