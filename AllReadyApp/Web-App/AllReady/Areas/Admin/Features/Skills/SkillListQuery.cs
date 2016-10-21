using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Skill;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    /// <summary>
    /// A query which returns all skills in the form of a <see cref="SkillSummaryViewModel"/>, including their descendant info. This excludes returning any skills with an invalid hierarchy
    /// </summary>
    public class SkillListQuery : IAsyncRequest<IEnumerable<SkillSummaryViewModel>>
    {
        /// <summary>
        /// If specified allows the query results to be limited to a single organization
        /// </summary>
        public int? OrganizationId { get; set; }
    }
}