using AllReady.Areas.Admin.ViewModels.Goal;
using MediatR;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalEditQuery : IAsyncRequest<GoalEditViewModel>
    {
        public int GoalId { get; set; }
    }
}
