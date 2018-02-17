namespace AllReady.ViewModels.Manage
{
    public class IndexViewModel
    {
        public ProfileViewModel ProfileViewModel { get; set; }
            = new ProfileViewModel();

        public SkillsViewModel SkillsViewModel { get; set; }
            = new SkillsViewModel();
    }
}