using System;
using AllReady.Areas.Admin.ViewModels.Request;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class EditRequestQuery : IAsyncRequest<EditRequestViewModel>
    {
        public Guid Id { get; set; }
    }
}
