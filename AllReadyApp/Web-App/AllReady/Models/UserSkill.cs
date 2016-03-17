namespace AllReady.Models
{
    public class UserSkill
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

    }
}
