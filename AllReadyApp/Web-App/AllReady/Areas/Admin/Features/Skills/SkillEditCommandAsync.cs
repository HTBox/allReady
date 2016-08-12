using AllReady.Areas.Admin.ViewModels.Skill;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditCommandAsync : IAsyncRequest<int>
    {
        public SkillEditModel Skill { get; set; }
    }
}