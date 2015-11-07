using AllReady.Areas.Admin.ViewModels;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskQuery : IRequest<TaskSummaryViewModel>
    {
        public int TaskId { get; set; }
    }
}
