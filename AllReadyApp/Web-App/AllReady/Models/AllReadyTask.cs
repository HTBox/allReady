using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
  public class AllReadyTask
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Tenant Tenant { get; set; }
    public Activity Activity { get; set; }
    public DateTime? StartDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }
    public List<TaskUsers> AssignedVolunteers { get; set; }
        public List<Skill> RequiredSkills { get; set; }
  }
}