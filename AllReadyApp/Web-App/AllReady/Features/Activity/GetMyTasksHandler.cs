using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyTasksHandler : IRequestHandler<GetMyTasksCommand, IEnumerable<TaskSignupViewModel>>
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public GetMyTasksHandler(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }

        public IEnumerable<TaskSignupViewModel> Handle(GetMyTasksCommand message)
        {
            var tasks = _allReadyDataAccess.GetTasksAssignedToUser(message.ActivityId, message.UserId);
            return tasks.Select(t => new TaskSignupViewModel(t));
        }
    }
}
