using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillDeleteCommand : IAsyncRequest
    {
        public int Id { get; set; }
    }
}