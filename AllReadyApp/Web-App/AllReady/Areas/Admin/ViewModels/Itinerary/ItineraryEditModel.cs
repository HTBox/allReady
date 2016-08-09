using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models.ItineraryModels
{
    public class ItineraryEditModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
