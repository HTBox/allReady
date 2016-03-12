using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyTasksCommand : IRequest<IEnumerable<TaskSignupViewModel>>
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
    }
}
