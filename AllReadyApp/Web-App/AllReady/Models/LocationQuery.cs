using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
  public class LocationQuery
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Nullable<int> Distance { get; set; }
    public Nullable<int> MaxRecordsToReturn { get; set; }
  }
}
