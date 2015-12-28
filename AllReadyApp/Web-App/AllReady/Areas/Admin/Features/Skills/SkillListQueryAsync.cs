using AllReady.Areas.Admin.Models;
using MediatR;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillListQueryAsync : IAsyncRequest<IEnumerable<SkillSummaryModel>>
    {
        /// <summary>
        /// If specified allows the query results to be limited to a single organization
        /// </summary>
        public int? OrganizationId { get; set; }
    }
}