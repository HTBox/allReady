using MediatR;
using System;
using AllReady.Areas.Admin.ViewModels.Request;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestSummaryQuery : IAsyncRequest<RequestSummaryModel>
    {
        public Guid RequestId { get; set; }
    }
}
