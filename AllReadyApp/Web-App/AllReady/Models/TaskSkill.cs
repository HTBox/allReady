namespace AllReady.Models
{
    public class TaskSkill
    {
        public int TaskId { get; set; }
        public virtual AllReadyTask Task { get; set; }

        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }

    }
}
