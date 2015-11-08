using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
  public class ActivitySignup
  {
    public int Id { get; set; }
    public ApplicationUser User { get; set; }
    public Activity Activity { get; set; }
    public DateTime SignupDateTime { get; set; }
    public DateTime? CheckinDateTime { get; set; }

  }
}
