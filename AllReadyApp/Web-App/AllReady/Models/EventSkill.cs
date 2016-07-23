namespace AllReady.Models
{
    public class EventSkill
    {
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
