using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyActivitiesQuery : IRequest<MyActivitiesResultsScreenViewModel>
    {
        public string UserId { get; set; }
    }
}
