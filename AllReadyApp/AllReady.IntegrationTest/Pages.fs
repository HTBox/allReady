module Pages
open canopy

module TopMenu =
    let private Campaigns = "Campaigns"
    let private Admin = "li.dropdown-admin"
    let private AdminCampaigns = "a[href='/Admin/Campaign']"
    let private AdminOrganizations = "a[href='/Admin/Organization']"

    let SelectAdminCampaigns _ =
        hover Admin
        click AdminCampaigns

    let SelectAdminOrganizations _ =
        hover Admin
        click AdminOrganizations

    let SelectCampaigns _ =
        click Campaigns

module AdminCampaigns =
    let RelativeUrl = "Admin/Campaign"
    let private CreateNew = "Create New"

    let SelectCreateNew _ = 
        click CreateNew
    
module AdminCampaignCreate =

    type CampaignDetails = {
        Name:string
        Description:string
        FullDescription:string
        OrganizationName:string
    }

    let DefaultCampaignDetails = {
        Name = ""
        Description = ""
        FullDescription = ""
        OrganizationName = ""
    }

    let private createBtn = "Create"
    let Submit _ =
        click createBtn

    let PopulateCampaignDetails details =
        "#Name" << details.Name
        "#Description" << details.Description
        press tab
        let insertFullDescriptionScript = sprintf "tinyMCE.activeEditor.setContent('%s')" details.FullDescription
        js(insertFullDescriptionScript) |> ignore
        "#OrganizationId" << details.OrganizationName


module AdminCampaignDetails =
    let RelativeUrl = "Admin/Campaign/Details"

    let private createNew = "Create New"

    let CreateNewActivity _ =
        click createNew



module AdminOrganizationCreate =
    type OrganizationDetails = {
        Name:string
        WebUrl:string
        LogoUrl:string
    }
    let DefaultOrganizationDetails  = {
        Name = ""
        WebUrl=""
        LogoUrl=""
    }

    let PopulateOrganizationDetails details =
        "#LogoUrl" << details.LogoUrl
        "#Name" << details.Name 
        "#WebUrl" << details.WebUrl

    let Save _ =
        click "Save"



module AdminActivityCreate =
    type ActivityDetails = {
        Name:string
        Description:string
        VolunteersRequired:int
        StartDate:string
        EndDate:string
        ActivityType: int
    }

    let DefaultActivityDetails = {
        Name = ""
        Description = ""
        VolunteersRequired = 1
        StartDate = ""
        EndDate = ""
        ActivityType = 1
    }

    let PopulateActivityDetails details =
        "#Name" << details.Name
        "#Description" << details.Description
        "#NumberOfVolunteersRequired" << details.VolunteersRequired.ToString()
        "#StartDateTime" << details.StartDate
        "#EndDateTime" << details.EndDate
        "#ActivityType" << details.ActivityType.ToString()

    let private createBtn = "Create"

    let Create _ =
        click createBtn

module AdminActivityDetails = 
    let RelativeUrl = "Admin/Activity/Details"