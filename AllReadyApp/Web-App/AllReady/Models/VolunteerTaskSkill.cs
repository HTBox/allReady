namespace AllReady.Models
{
    public class VolunteerTaskSkill
    {
        public int VolunteerTaskId { get; set; }
        public virtual VolunteerTask VolunteerTask { get; set; }

        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
