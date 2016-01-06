module CreatingOrgsAndCampaigns

open canopy
open runner
open System
open Pages

let All baseUrl =
    let testOrganizationName = Utils.GetScenarioTestName "My test project"
    let testCampaignName = Utils.GetScenarioTestName "My test campaign"
 
    context "Creating Orgs And Campaigns"

    "Login Admin" &&& fun _ ->
        Actions.GoToHomePage baseUrl
        let sceenshotName  = Utils.TimestampName "HomePage"
        screenshot "c:\screenshots" sceenshotName |> ignore
        Actions.Login Constants.SiteAdminUserName Constants.SiteAdminPassword baseUrl

    "Admin can Navigate to Organizations" &&& fun _ ->
        TopMenu.SelectAdminOrganizations()

        "h2" == "Currently active organizations"
        title() |> is "Currently active organizations - AllReady"

    "Admin can create organization" &&& fun _ ->
        click "Create New"
        AdminOrganizationCreate.PopulateOrganizationDetails 
            {AdminOrganizationCreate.DefaultOrganizationDetails with Name = testOrganizationName;WebUrl="htbox.org"}
        AdminOrganizationCreate.Save()

        "td a" *= testOrganizationName

    "Admin can navigate to campaigns" &&& fun _ ->
        TopMenu.SelectAdminCampaigns()

        "h2" == "Campaigns - Admin"
        title() |> is "Campaigns - Admin - AllReady"

    "Admin can create new campaign" &&& fun _ ->
        AdminCampaigns.SelectCreateNew()
        AdminCampaignCreate.PopulateCampaignDetails 
            {AdminCampaignCreate.DefaultCampaignDetails with 
                Name = testCampaignName; 
                Description = "test"; 
                FullDescription = "Full Description"; 
                OrganizationName = testOrganizationName}
        AdminCampaignCreate.Submit()

        "h2" == testCampaignName
        on AdminCampaignDetails.RelativeUrl
        TopMenu.SelectCampaigns()
        "td a" *= testCampaignName


    lastly(fun _ ->
        click "i.fa-sign-out"
    )