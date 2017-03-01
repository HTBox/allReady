using AllReady.Areas.Admin.ViewModels.Goal;
using MediatR;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalEditCommand : IAsyncRequest<int>
    {
        public GoalEditViewModel Goal { get; set; }
    }
}
