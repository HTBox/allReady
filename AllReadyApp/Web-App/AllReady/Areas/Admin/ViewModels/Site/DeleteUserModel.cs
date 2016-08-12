using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Site
{
    public class DeleteUserModel
    {
        public string UserId { get; set; }
        [Display(Name = "User name")]
        public string UserName { get; set; }

    }
}