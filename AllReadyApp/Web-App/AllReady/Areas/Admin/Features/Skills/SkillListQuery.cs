using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Skill;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillListQuery : IAsyncRequest<IEnumerable<SkillSummaryViewModel>>
    {
        /// <summary>
        /// If specified allows the query results to be limited to a single organization
        /// </summary>
        public int? OrganizationId { get; set; }
    }
}