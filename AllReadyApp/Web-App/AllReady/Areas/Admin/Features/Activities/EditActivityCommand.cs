using AllReady.Areas.Admin.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class EditActivityCommand : IRequest<int>
    {
        public ActivityDetailViewModel Activity {get; set;}
    }
}
