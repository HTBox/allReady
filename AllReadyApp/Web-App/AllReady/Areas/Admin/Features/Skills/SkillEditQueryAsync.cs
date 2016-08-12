using AllReady.Areas.Admin.ViewModels.Skill;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditQueryAsync : IAsyncRequest<SkillEditModel>
    {
        public int Id { get; set; }
    }
}