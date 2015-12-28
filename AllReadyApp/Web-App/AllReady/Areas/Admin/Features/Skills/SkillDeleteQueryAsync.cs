using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillDeleteQueryAsync : IAsyncRequest<SkillDeleteModel>
    {
        public int Id { get; set; }
    }
}