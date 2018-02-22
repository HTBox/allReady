module Pages
open canopy

module TopMenu =
    let private Campaigns = "Campaigns"
    let private Admin = "li.dropdown-admin a"
    let private AdminCampaigns = "a[href='/Admin/Campaign']"
    let private AdminOrganizations = "a[href='/Admin/Organization']"

    let SelectAdminCampaigns _ =
        hover Admin
        sleep 1
        click AdminCampaigns

    let SelectAdminOrganizations _ =
        hover Admin
        sleep 1
        click AdminOrganizations

    let SelectCampaigns _ =
        click Campaigns

module AdminOrganizations =
    let RelativeUrl = "Admin/Organization"

module Campaigns = 
    let RelativeUrl = "Campaign"
    let private CreateNew = "Create Campaign"

    let SelectCreateNew _ = 
        click CreateNew

module AdminCampaigns =
    let RelativeUrl = "Admin/Campaign"
    let private CreateNew = "#CreateNew"

    let SelectCreateNew _ = 
        click CreateNew
    
module AdminCampaignCreate =

    type CampaignDetails = {
        Name:string
        Description:string
        Headline:string
        FullDescription:string
        OrganizationName:string
        Address1:string
        City:string
        State:string
        PostalCode:int
        Country:string
    }

    let DefaultCampaignDetails = {
        Name = ""
        Description = ""
        FullDescription = ""
        Headline = ""
        OrganizationName = ""
        Address1="1 Microsoft Way"
        City="Redmond"
        State="WA"
        PostalCode=98052
        Country="US"
    }

    let private createBtn = "Create"
    let Submit _ =
        click createBtn

    let PopulateCampaignDetails details =
        "#Name" << details.Name
        "#Description" << details.Description
        "#Headline" << details.Headline
        press tab
        let insertFullDescriptionScript = sprintf "tinyMCE.activeEditor.setContent('%s')" details.FullDescription
        js(insertFullDescriptionScript) |> ignore
        "#OrganizationId" << details.OrganizationName
        check "#Published"
        "#Location_Address1" << details.Address1
        "#Location_City" << details.City
        "#Location_State" << details.State
        "#Location_PostalCode" << details.PostalCode.ToString()
        "#Location_Country" << details.Country

module AdminCampaignDetails =
    let RelativeUrl = "Admin/Campaign/Details"

    let private createNew = "a[href^='/Admin/Event/Create/']"

    let CreateNewEvent _ =
        click createNew

module AdminOrganizationCreate =
    let privicyPolicyFieldVisible () =
        (elements "#pp-url").Length = 1

    type OrganizationDetails = {
        Name:string
        WebUrl:string
        LogoUrl:string
        Address1:string
        City:string
        State:string
        PostalCode:int
        Country:string
        PrivacyPolicyUrl:string
    }
    let DefaultOrganizationDetails  = {
        Name = ""
        WebUrl=""
        LogoUrl=""
        Address1="1 Microsoft Way"
        City="Redmond"
        State="WA"
        PostalCode=98052
        Country="US"
        PrivacyPolicyUrl="http://putsomethinghere.com"
    }

    let PopulateOrganizationDetails details =
        "#LogoUrl" << details.LogoUrl
        "#Name" << details.Name 
        "#WebUrl" << details.WebUrl
        "#Location_Address1" << details.Address1
        "#Location_City" << details.City
        "#Location_State" << details.State
        "#Location_PostalCode" << details.PostalCode.ToString()
        "#Location_Country" << details.Country
        click "Link to an external policy"
        waitFor privicyPolicyFieldVisible
        "#PrivacyPolicyUrl" << details.PrivacyPolicyUrl

    let Save _ =
        click "Create"


module AdminEventCreate = 
    
    type EventDetails = {
        Name:string
        StartDate:System.DateTime
        EndDate:System.DateTime
        EventType: int
        City:string
        State:string
        PostalCode:int
        Country:string
        Address1:string
    }

    let DefaultEventDetails = {
        Name = ""
        StartDate = System.DateTime.Now.AddDays(1.0)
        EndDate = System.DateTime.Now.AddDays(5.0)
        EventType = 2
        City="Redmond"
        State="WA"
        PostalCode=98052
        Country="US"
        Address1="Address Goes Here"
    }

    let PopulateEventdetails details =
        "#Name" << details.Name
        "#StartDateTime" << details.StartDate.ToString("MM/dd/yyyy")
        "#EndDateTime" << details.EndDate.ToString("MM/dd/yyyy")
        "#EventType" << details.EventType.ToString()
        "#Location_City" << details.City
        "#Location_State" << details.State
        "#Location_PostalCode" << details.PostalCode.ToString()
        "#Location_Country" << details.Country
        "#Location_Address1" << details.Address1

    let private createBtn = ".submit-form"

    let Create _ =
        click createBtn

module AdminTaskCreate =
    type TaskDetails = {
        Name:string
        Description:string
        VolunteersRequired:int
        StartDate:System.DateTime
        EndDate:System.DateTime
    }

    let DefaultTaskDetails = {
        Name = ""
        Description = ""
        VolunteersRequired = 1
        StartDate = System.DateTime.Now.AddDays(1.0)
        EndDate = System.DateTime.Now.AddDays(4.0)
    }

    let PopulateTaskDetails details =
        "#Name" << details.Name
        "#Description" << details.Description
        "#NumberOfVolunteersRequired" << details.VolunteersRequired.ToString()
        "#StartDateTime" << details.StartDate.ToString("MM/dd/yyyy h:mm tt")
        "#EndDateTime" << details.EndDate.ToString("MM/dd/yyyy h:mm tt")

    let private createBtn = "Save"

    let Create _ =
        click createBtn

module AdminEventDetails = 
    let RelativeUrl = "Admin/Event/Details"

    let private createNew = "Create Task"

    let CreateNewTask _ =
        click createNew

module AdminTaskDetails = 
    let RelativeUrl = "Admin/Task/Details"


