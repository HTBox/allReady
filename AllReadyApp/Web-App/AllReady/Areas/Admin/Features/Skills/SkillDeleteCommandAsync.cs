using MediatR;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillDeleteCommandAsync : IAsyncRequest
    {
        public int Id { get; set; }
    }
}