using AllReady.Areas.Admin.ViewModels.Goal;
using MediatR;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalDeleteQuery : IAsyncRequest<GoalDeleteViewModel>
    {
        public int GoalId { get; set; }
    }
}
