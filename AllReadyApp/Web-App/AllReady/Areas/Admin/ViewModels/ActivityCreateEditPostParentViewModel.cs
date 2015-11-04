using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class ActivityCreateEditPostParentViewModel
    {
        public bool IsCreateView { get; set; } = true;

        public string PageTitle { get; set; } = "Create Activity";

        public int ActivityId { get; set; }

        public string ActivityName { get; set; }
        
        public int CampaignId { get; set; }

        public string CampaignName { get; set; }

        public ActivityCreateEditViewModel CreateEditViewModel { get; set; }

        public ActivityFileUploadViewModel UploadFileViewModel { get; set; }
    }
}
