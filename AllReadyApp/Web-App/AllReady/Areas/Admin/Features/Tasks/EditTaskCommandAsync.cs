﻿using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskCommandAsync : IAsyncRequest<int>
    {
        public EditViewModel Task { get; set; }
    }
}
