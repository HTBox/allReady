using System;

namespace AllReady.Areas.Admin.Models.ItineraryModels
{
    public class ItineraryListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int TeamSize { get; set; }
        public int RequestCount { get; set; }

        public string DisplayDate => Date.ToLongDateString();
    }
}
