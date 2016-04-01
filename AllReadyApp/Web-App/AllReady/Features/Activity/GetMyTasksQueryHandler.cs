using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyTasksQueryHandler : IRequestHandler<GetMyTasksQuery, IEnumerable<TaskSignupViewModel>>
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public GetMyTasksQueryHandler(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }

        public IEnumerable<TaskSignupViewModel> Handle(GetMyTasksQuery message)
        {
            var tasks = _allReadyDataAccess.GetTasksAssignedToUser(message.ActivityId, message.UserId);
            return tasks.Select(t => new TaskSignupViewModel(t));
        }
    }
}
