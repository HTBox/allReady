using AllReady.Areas.Admin.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class DeleteActivityCommand : IRequest
    {
        public int ActivityId {get; set;}
    }
}
