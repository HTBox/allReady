using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepOps.Models
{
    public class PrepOpsTask
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Tenant Tenant { get; set; }

        public Activity Activity { get; set; }

        public DateTime? StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public List<TaskUsers> AssignedVolunteers { get; set; }

    }
}