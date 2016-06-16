namespace AllReady.ViewModels.Home
{
    public class CampaignSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Models.Organization Organization { get; set; }
    }
}