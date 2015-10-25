using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
  public class ClosestLocation
  {
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public double Distance { get; set; }
  }
}
