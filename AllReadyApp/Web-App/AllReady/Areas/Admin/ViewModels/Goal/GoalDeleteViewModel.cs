using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Goal
{
    public class GoalDeleteViewModel
    {
        public int Id { get; set; }
        public GoalType GoalType { get; set; }
        public string TextualGoal { get; set; }
        public int NumericGoal { get; set; }

        public int OwningOrganizationId { get; set; }
        public int CampaignId { get; set; }
    }
}