using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using MediatR;

namespace AllReady.Areas.Admin.Features.Users
{
    /// <summary>
    ///     List volunteers that we can assign to tasks.
    /// </summary>
    public class ListPossibleVolunteersForTaskQuery : IAsyncRequest<IReadOnlyList<VolunteerSummary>>
    {
        /// <summary>
        ///     The voluteers should have previously volunteered for tasks in this organization.
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        ///     Exclude users that already have been assigned to the specified task.
        /// </summary>
        public int TaskId { get; set; }
    }
}
