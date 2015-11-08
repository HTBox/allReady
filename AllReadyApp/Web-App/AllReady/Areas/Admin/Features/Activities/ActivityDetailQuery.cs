using AllReady.Areas.Admin.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class ActivityDetailQuery : IRequest<ActivityDetailModel>
    {
        public int ActivityId { get; set; }
    }
}
