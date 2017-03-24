using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Resource;
using MediatR;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class DeleteResourceQuery : IAsyncRequest<ResourceDeleteViewModel>
    {
        public int ResourceId { get; set; } 
    }
}
