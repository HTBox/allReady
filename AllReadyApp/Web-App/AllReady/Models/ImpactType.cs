using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public enum ImpactType
    {
        [Display(Name="Numeric Impact")]
        Numeric = 0,
        [Display(Name="Textual Impact")]
        Text = 1
    }
}
