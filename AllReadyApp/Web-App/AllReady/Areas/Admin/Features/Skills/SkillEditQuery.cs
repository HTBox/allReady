using AllReady.Areas.Admin.ViewModels.Skill;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditQuery : IAsyncRequest<SkillEditViewModel>
    {
        public int Id { get; set; }
    }
}