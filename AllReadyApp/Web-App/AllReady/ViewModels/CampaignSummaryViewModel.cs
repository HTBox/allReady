using AllReady.Models;
using System;

namespace AllReady.ViewModels
{
    public class CampaignSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Organization Organization { get; set; }   
    }
}