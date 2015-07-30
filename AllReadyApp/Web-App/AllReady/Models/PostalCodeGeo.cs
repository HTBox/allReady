using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{

    public class PostalCodeGeo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PostalCode { get; set; }

        public string City { get; set; }
        public string State { get; set; }
    }
}
