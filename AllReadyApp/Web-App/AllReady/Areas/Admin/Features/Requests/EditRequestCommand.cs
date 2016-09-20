using System;
using AllReady.Areas.Admin.ViewModels.Request;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class EditRequestCommand : IAsyncRequest<Guid>
    {
        public EditRequestViewModel RequestModel { get; set; }
    }
}
