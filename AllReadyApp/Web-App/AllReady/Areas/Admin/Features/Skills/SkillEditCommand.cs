using AllReady.Areas.Admin.ViewModels.Skill;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditCommand : IAsyncRequest<int>
    {
        public SkillEditViewModel Skill { get; set; }
    }
}