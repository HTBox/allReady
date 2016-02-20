using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ShowActivityCommand : IRequest<ActivityViewModel>
    {
        public int ActivityId { get; set; }
    }
}