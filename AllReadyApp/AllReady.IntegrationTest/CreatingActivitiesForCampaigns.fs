module CreatingActivitiesForCampaigns

open canopy
open runner
open System
open Pages

let All baseUrl =


    context "Organization Admin Activities"

    "Login Org Admin" &&& fun _ ->
        Actions.GoToHomePage baseUrl
        Actions.Login Constants.OrgAdminUserName Constants.OrgAdminPassword baseUrl

    "The Org Admin can navigate to the admin campaigns page" &&& fun _ ->
        TopMenu.SelectAdminCampaigns()

        on AdminCampaigns.RelativeUrl
        "h2" == "Campaigns - Admin"
        title() |> is "Campaigns - Admin - allReady"

    "The Org Admin can create a campaign" &&& fun _ ->
        let testCampaignName = Utils.GetScenarioTestName "Test Campaign for Org Admin"
        AdminCampaigns.SelectCreateNew()
        AdminCampaignCreate.PopulateCampaignDetails
            {AdminCampaignCreate.DefaultCampaignDetails with
                Name = testCampaignName;
                Description = "test";
                Headline = "test";
                FullDescription = "Full Description";
                OrganizationName = "Humanitarian Toolbox"}
        AdminCampaignCreate.Submit()

        on AdminCampaignDetails.RelativeUrl
        "h2" == testCampaignName

        TopMenu.SelectCampaigns()
        "td a" *= testCampaignName

        navigate(back)

// Reference for eventNameSelector: http://www.jeremybellows.com/blog/Writing-Quality-Css-Selectors-for-Canopy-UI-Automation
    "The Org Admin can create events" &&& fun _ ->
        let eventName = Utils.GetScenarioTestName "First Test Event"
        let eventNameSelector = "[data-event-title]"

        AdminCampaignDetails.CreateNewEvent()
        AdminEventCreate.PopulateEventdetails
            {AdminEventCreate.DefaultEventDetails with
                Name = eventName
            }
        AdminEventCreate.Create()

        on AdminEventDetails.RelativeUrl

        let eventNameValue = read <| element eventNameSelector
        eventNameValue |> contains eventName

    "The Org Admin can create task" &&& fun _ ->
        let taskName = Utils.GetScenarioTestName "First Test Task"
        AdminEventDetails.CreateNewTask()
        AdminTaskCreate.PopulateTaskDetails
            {AdminTaskCreate.DefaultTaskDetails with
                Name = taskName
            }

        AdminTaskCreate.Create()

        on AdminEventDetails.RelativeUrl
        "td a" *= taskName

    "The Org Admin can logout" &&& fun _ ->
        hover ".dropdown-account"
        sleep 1
        click ".log-out"

