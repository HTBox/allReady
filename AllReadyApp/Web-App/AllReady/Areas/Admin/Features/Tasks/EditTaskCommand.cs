using AllReady.Areas.Admin.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskCommand : IRequest<int>
    {
        public TaskEditModel Task {get; set;}
    }
}
