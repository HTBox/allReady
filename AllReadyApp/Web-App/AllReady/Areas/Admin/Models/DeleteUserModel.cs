using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class DeleteUserModel
    {
        public string UserId { get; set; }
        [Display(Name = "User name")]
        public string UserName { get; set; }

    }
}