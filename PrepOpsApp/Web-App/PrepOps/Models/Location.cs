using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepOps.Models
{
    public class Location
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public PostalCodeGeo PostalCode { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Country { get; set; } = "USA";
    }
}