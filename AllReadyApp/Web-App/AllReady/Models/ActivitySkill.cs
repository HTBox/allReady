namespace AllReady.Models
{
    public class ActivitySkill
    {
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

    }
}
