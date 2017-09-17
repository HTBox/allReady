namespace AllReady.Models
{
    public class VolunteerTaskSkill
    {
        public int VolunteerTaskId { get; set; }
        public VolunteerTask VolunteerTask { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
