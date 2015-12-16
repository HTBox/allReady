using AllReady.Areas.Admin.Models;
using MediatR;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class TenantSkillListQueryAsync : IAsyncRequest<IEnumerable<SkillSummaryModel>>
    {
        public int Id { get; set; }
    }
}