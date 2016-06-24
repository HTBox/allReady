using AllReady.Areas.Admin.Models.RequestModels;
using MediatR;
using System;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class RequestSummaryQuery : IAsyncRequest<RequestSummaryModel>
    {
        public Guid RequestId { get; set; }
    }
}
