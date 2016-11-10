using AllReady.Areas.Admin.ViewModels.Skill;
using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    /// <summary>
    /// A query which returns a <see cref="SkillEditViewModel"/> for a specified skill id
    /// </summary>
    public class SkillEditQuery : IAsyncRequest<SkillEditViewModel>
    {
        /// <summary>
        /// The Id of the skill to be edited
        /// </summary>
        public int Id { get; set; }
    }
}