using AllReady.Areas.Admin.Models.RequestModels;
using MediatR;
using System;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestSummaryQuery : IAsyncRequest<RequestSummaryModel>
    {
        public Guid RequestId { get; set; }
    }
}
