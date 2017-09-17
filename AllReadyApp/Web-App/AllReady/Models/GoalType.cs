using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public enum GoalType
    {
        [Display(Name="Numeric Goal")]
        Numeric = 0,
        [Display(Name="Textual Goal")]
        Text = 1
    }
}
