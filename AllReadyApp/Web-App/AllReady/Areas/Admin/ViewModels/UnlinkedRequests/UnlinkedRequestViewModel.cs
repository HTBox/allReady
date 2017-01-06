using System;

namespace AllReady.Areas.Admin.ViewModels.UnlinkedRequests
{
    public class UnlinkedRequestViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
