using System;

namespace AllReady.Models
{
  public class LocationQuery
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? Distance { get; set; }
    public int? MaxRecordsToReturn { get; set; }
  }
}
