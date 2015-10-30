using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
  public class Resource
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime PublishDateBegin { get; set; }
    public DateTime PublishDateEnd { get; set; }
    public string MediaUrl { get; set; }
    public string ResourceUrl { get; set; }
    public string CategoryTag { get; set; }
  }
}
