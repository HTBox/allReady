using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
  public class TaskUsers
  {
    public int Id { get; set; }
    public AllReadyTask Task { get; set; }
    public ApplicationUser User { get; set; }
    public DateTime StatusDateTimeUtc { get; set; } = DateTime.UtcNow;
    public string Status { get; set; }
    public string StatusDescription { get; set; }
  }
}