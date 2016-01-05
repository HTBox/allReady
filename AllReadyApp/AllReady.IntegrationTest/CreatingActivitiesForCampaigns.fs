module CreatingActivitiesForCampaigns

open canopy
open runner
open System
open Pages

let All baseUrl =
    
    let testCampaignName = Utils.GetScenarioTestName "Test Campaign for Org Admin"

    context "Organization Administration Activities"

    "Login Org Admin" &&& fun _ ->
        Actions.GoToHomePage baseUrl
        Actions.Login Constants.OrgAdminUserName Constants.OrgAdminPassword baseUrl

    "The Org Admin can navigate to the admin campaigns page" &&& fun _ ->
        TopMenu.SelectAdminCampaigns()
        
        on AdminCampaigns.RelativeUrl

    "The Org Admin can create a campaign" &&& fun _ ->
        AdminCampaigns.SelectCreateNew()
        AdminCampaignCreate.PopulateCampaignDetails 
            {AdminCampaignCreate.DefaultCampaignDetails with 
                Name = testCampaignName; 
                Description = "test"; 
                FullDescription = "Full Description"; 
                OrganizationName = "Humanitarian Toolbox"}
        AdminCampaignCreate.Submit()

        "h2" == testCampaignName

    "The Org Admin can create activies" &&& fun _ -> 
        let activityName = Utils.GetScenarioTestName "First Test Activity"
        AdminCampaignDetails.CreateNewActivity()
        AdminActivityCreate.PopulateActivityDetails 
            {AdminActivityCreate.DefaultActivityDetails with 
                Name = activityName
                StartDate = Dates.Tomorrow()
                EndDate = Dates.OneWeekFromToday()
            }
        AdminActivityCreate.Create()

        on AdminActivityDetails.RelativeUrl
        "h2" == activityName

