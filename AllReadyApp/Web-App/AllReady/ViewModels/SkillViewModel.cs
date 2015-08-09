using System;

namespace AllReady.ViewModels
{
    public class SkillViewModel
    {
        public int Id { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberRequired { get; set; }
        public string LocationNeeded { get; set; }
    }
}
