using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyActivitiesCommand : IRequest<MyActivitiesResultsScreenViewModel>
    {
        public string UserId { get; set; }
    }
}
