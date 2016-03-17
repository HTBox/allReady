using System.ComponentModel.DataAnnotations;

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
