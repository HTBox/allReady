using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    public abstract class Capability
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDateTimeUtc { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDateTimeUtc { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Number required")]
        public int NumberRequired { get; set; }

        //TODO: Change this from string to Location
        [Display(Name = "Location needed")]
        public string LocationNeeded { get; set; }
    }
}
