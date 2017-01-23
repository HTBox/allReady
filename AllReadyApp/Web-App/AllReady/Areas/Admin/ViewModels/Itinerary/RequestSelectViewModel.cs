namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class RequestSelectViewModel : RequestListViewModel
    {
        public bool IsSelected { get; set; }

        public string DateAddedString => DateAdded.ToString("D");
    }
}
