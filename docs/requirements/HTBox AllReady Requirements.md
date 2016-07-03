*[Status: v1.1 updated draft to define the next stage MVP for the "Go Live" release to support the June 11th Northwest Region of the American Red Cross preparedness event which anticipates getting smoke alarms installed into over 1,250 homes across Washington state.  Also updated to allReady casing and other minor notes below. We will focus on the key MVP needs for this release and then generalize to other use cases after the June 11th release]*

*[Status: v0.9 updated draft with additional messaging, increasing detail on requirements and first pass at a scope for an initial mvp feature set as well as rename to AllReady and associated cleanup] Tony Surma 2015-08-08]*

*Note: TBD = To be defined in this requirements document.  Many of these items are understood or partially captured but not yet codified as requirements in this document (and thus not also issues in github repository)*

HTBox AllReady Application Requirements
=======================================

## 1 CONTEXT

Preparedness is an increasingly key area of focus and need for humanitarian organizations as they look to reduce the impact of crises and disasters.  Several different reports and metrics exist in the industry to show that impact of $1 or 1 hour prior to a disaster is equivalent to 20x to 30x that after a disaster.  Therefore despite the natural allure of the 'heroism' of response to volunteers and the media, higher impact and results can be achieved by focusing on preparedness campaigns.  In order to engage volunteers, donors, organizations and individual citizens in preparedness work, campaigns must be visible and discoverable; accessible to the citizens they are targeting; transparent in how goals, progress and results are communicated; and be predictably executed with similar rigor and focus as response to individual crises.

Ultimately, the aspirational goal of preparedness campaign delivery is to 'drive disaster response services out of business' by fully preparing and equipping communities and individuals to significantly reduce the impact of personal and large scale disasters if not fully stop them from happening in the first place.

## 2 MESSAGING/STORY

### 2.1 MESSAGING OPTIONS

Below are various messaging options that vary by length and target audience

#### 2.1.1 Connecting communities and organization in increasing preparedness

#### 2.1.2 Connecting communities and organizations in delivering preparedness campaigns openly and effectively

#### 2.1.3 Helping non-governmental organizations deliver preparedness campaigns to engage communities in lower impact potential of disasters.

#### 2.1.4 Creating visibility and engagement for organizations with the communities they serve as they design and deliver preparedness campaigns

#### 2.1.5 Creating open visibility into the preparedness campaigns and effectiveness of organizations serving for citizens' communities

### 2.2 METAPHOR REFERENCE

AllReady is focused on Preparedness Operations and is inspired by the fable of the Ants and the Grasshopper.  In the fable (or at least some of the many variants), the ants work hard throughout the Autumn to prepare for the pending winter.  Together they gather food and build shelter and ensure that they are ready to the meet the challenges of the winter months.  Meanwhile the grasshopper spends its days idle and at play without a care for the upcoming winter.  As winter comes the ants are prepared and able to survive the winter but the grasshopper is not.  Like the ants, communities, organizations and volunteers are stronger together when engaged, connected and working in concert and through preparedness they are able to lessen the impact of disasters both big and small.

## 3 APPLICATION GOALS

The AllReady application is the platform on which organizations can easily create, communicate, deliver impact and report on preparedness campaigns to increase their visibility, impact, efficiency and transparency.

In detail the application will allow for these high level functional areas & technical attributes which will drive the desired outcomes as documented.

## 3.1 FUNCTIONAL AREAS

#### 3.1.1 Create

Organizations utilize the application to create, manage and publish preparedness campaigns that capture content and information for public communication, impact metrics goals and progress, geographic and timeline goals, volunteer tasks and activities, data and results gathered by volunteers and requests and results provided by individuals in the communities served.

By centralizing this process the full campaign can be managed by individual organizations and a joint community can be created around all preparedness campaigns being delivered in geographies that will be both accessible by community members as well as create a wealth of data that – with the proper security and privacy controls – can be analyzed together to increase coordination and effectiveness of all campaigns and their impact on communities.

#### 3.1.2 Communicate

Engagement of members of the local community, involvement of diverse groups of volunteers and simple awareness that a campaign is active are key to the effectiveness of preparedness campaigns.  The application will allow for communication of the content and information about a campaign to be discovered by others through a central public website of campaigns searchable by text, geography and focus area (e.g. fire safety, nutritional assistance, etc.) as well as be linked to directly within an organizations' website to a specific campaign or all campaigns for that organization.

Beyond broad communication, the application will also allow a 'self service' model for individuals in the community to 'sign up' to request the impact of the campaign and/or 'select out' of the campaign which both allows direct engagement of individuals as well as creates a more informed and directed execution of the campaign to target individuals and areas with greatest interest and need (of course, this will also be supplemented with the many other factors organizations use to prioritize and plan delivery).

Additionally, communication to past and potential volunteers (both individuals as well as other partner non-profit, faith-based, government or private sector organizations) will provide a clear understanding of the goals, the skills/capabilities needed, tasks that volunteers will be asked to deliver and direct means to engage on the campaign.  Research and experimentation with this type of communication with volunteers has shown increased engagement because they can predictably see the shape and size of the request and the impact it will have.

Lastly as detailed further in 2.1.4 'Report' below, the application will allow broad summary level communication of the goals, progress to date and effectiveness of the campaign to transparently show the impact and garner support for ongoing delivery of the campaign.  Logically a campaign will be able to show a 'thermometer' style visualization of impact to date against goals similarly seen in fundraising or IndieGoGo/Kickstarter type campaigns.

#### 3.1.3 Deliver Impact

Simply put, a preparedness campaign does nothing if the desired impact is not delivered.  The application will allow organization campaign operations staff to coordinate and direct the tasks of volunteers in communities; request/track/confirm the matching of specific tasks to volunteers; gather data from the volunteer and community member after the delivery occurs; gain insights into which tasks/volunteer/community combinations are most impactful; and ultimately deliver more impact with less resources quicker.  This operational aspect ensures that the delivery is as managed as the communications and the two together allow for analyzing and reporting on impact within an organization and transparently to the broader community around the campaign.

#### 3.1.4 Report

Donors, volunteers, and community members all want to know and see what a campaign is doing and what impact it is having.  Reporting on impact allows an organization to show results transparently and openly which drives increased volunteering and donation – increasingly even in small 'donations' people require visibility into how the investment of their time and money will drive direct outcomes.

So many organizations do so much good in preparedness but it either goes unseen by communities or it is told randomly through a few anecdotal stories in blog posts or social media bits and pieces.  By providing transparent reporting visualized geographically (e.g. heatmaps), across timelines (showing progress over time on goals and community geography), and on impact dashboards (including thermometer style and other formats presenting impact against plan and predicted outcomes) all stakeholders in the campaign can access, consume and understand the value of the campaign and derive insights into how and why they can contribute or engage further.

Also, reporting on operational delivery provides visibility to the organization delivering the campaign that can drive decisions that increase the delivery of campaigns during execution as well as shape the strategy and evaluation of future campaigns within that organization and across partnerships.

### 3.2 SOLUTION TECHNICAL ATTRIBUTES

The solution must be delivered with the following technical attributes to provide for delivery of the desired outcomes.

#### 3.2.1 Centralized Web Application

[TBD: Add more] The primary interface for community members, volunteers, organization staff and public access will be a centralized web application provided both CMS and management interfaces as well as public content access and engagement.  This centralization will allow a central deployment by HTBox within Azure to provide the capabilities to organizations and users without technical deployment barriers as well as drive the 'network effect' of having campaign and content accessible in a single location and insights from analysis of common data across campaigns.

#### 3.2.2 Volunteer Mobile Application

[TBD: Add more] Task management and data gathering by volunteers should be provided by a mobile application deployable across all major platforms (iOS, Android, Windows) that can operate in an offline/cached mode to allow for connectivity failures (along with a mobile friendly web application.)  Additionally most 'simple' task assignment and management features should be able to be completed via text message/SMS for rapid engagement with volunteers who have yet to download the app.

#### 3.2.3 Operations Staff Application

[TBD: Add more] Operations management and task/volunteer management should be delivered primarily by the main web application but ideally would also be provided as a mobile application which can offer more task oriented efficient operations for staff that may not be at a full desktop browser.

#### 3.2.4 Community Member Feedback Application

[TBD: Add more] Gathering of simple member feedback post delivery should be accessible both by a mobile friend web application as well as a SMS/text message interface to allow for greatest user access.  Ideally the application would also be accessible via IVR to accommodate those who may not have mobile devices.

## 4 APPLICATION DESIRED OUTCOMES & SUPPORTING FUNCTIONALITY

#### 4.1.1 Visibility

Campaigns are made discoverable, visible and actionable as follows:

a) Campaign content for marketing & communications is gathered from and maintained by the owning organization using a simple CMS like interface with fields including logos and titles for organization and campaign, media and textual body content, links to organizations and other web pages for reference, contact information, high level impact goals and measures (1-2), and volunteer needs & opportunities.

b) Campaigns can be published, edited and 'closed out' from the same CMS like interface

c) Campaign content such as impact goals and measures are calculated from execution without manual intervention

d) Campaign metadata is searchable and 'listable' including organization, focus area, geography served, text content and active/inactive status.

e) Campaigns are searchable and discoverable per (d) above both centrally in the hosted application as well as individually (or by organization) for linking/display from organization websites

f) Campaigns have appropriate forms (driven by data provided by the organization) for community members to request or decline outcomes of the campaign by providing contact information and simple survey information according to data provided by organization

g) Campaigns have appropriate forms (driven by data provided by the organization) for potential volunteers (as well as those who have volunteered previously) to volunteer themselves or organizations they represent to support the campaign by providing contact information and simple survey information according to data provided by organization

h) Reporting information (for active campaigns) is visible on campaign home page in highly summarized view (potentially thermometer etc.) and then able to be clicked into for deeper viewing of impact in the means described elsewhere in the requirements.

#### 4.1.2 Impact

#### 4.1.3 Efficiency

Operational execution of campaigns is managed through the application as follows:

a) Campaign operations staff can define tasks, steps of execution, and data gathering requirements for the operational aspects of a campaign

b) Campaign operations staff can search, identify candidate volunteers, request engagement, verify the volunteer acceptance and track execution of tasks by volunteers

c) Volunteers can be contacted and interact with task requests via app, web and sms interfaces

d) Campaign operations staff can see data gathered by volunteers and request feedback from community members served as tasks are completed.

e) Data gathering can be completed by volunteers by app and web interfaces and feedback from community members can be completed by web and sms interfaces

f) Campaign operations staff can reassign, cancel and otherwise manage the tasks being completed by volunteers.

g) Campaign staff can access and see volunteer history, response rate and other operational metrics to optimize their efficiency when assigning tasks to volunteers

h) Overtime additional machine learning or data analysis can be applied to suggest optimal volunteer, task, geography, etc. matches

#### 4.1.4 Transparency

With appropriate and requisite security, privacy and data scrubbing/summarization controls, all engagement, execution, operational delivery and outcome data is captured and maintained throughout the application to drive transparent reporting of impact and outcomes as well as increase operational efficiency as follows:

a) [TBD: Reporting needs to be detailed beyond and in alignment with what is mentioned elsewhere]

## 5 MINIMAL VIABLE PRODUCT SCOPE AND FIRST ITERATION

### 5.1 "V1" MVP

Understanding the short timeline for the initial development sprint starting in late June/early July and trying to go live in Oct/Nov timeframe as well as knowing that we want to have a representative MVP scope that can both show initial value as well as be an 'ambassador' for future value and further development the scope below represents a view of MVP scope for the July "V1 release".

**As adjusted during development we are consider this initial release "complete and delivered" as it has been used in exercises and tested out.  We are now focused on the next "Go Live" release as described below in section 6**

#### 5.1.1 Centralized Web Application

#### 5.1.1.1 Citizen Functionality

5.1.1.1.1 Allow citizens to access the web application anonymously w/o requiring authentication

5.1.1.1.2 Allow citizens to search for campaigns by organization, keyword(s), and geography covered (zipcode for now)

5.1.1.1.3 Allow citizens to view campaign details as entered by organization as well as deliver to date vs goals information and targets (e.g. 1500 / 39000 smoke detectors installed & plan to install ~2K per week until end of 2016)

5.1.1.1.4 Allow citizens to click through to links to other websites with related information as provided by organization [later functionality will include direct 'request/decline' interaction]

5.1.1.1.5 Allow citizens to share campaign information via email, txt, twitter and facebook

#### 5.1.1.2 Volunteer Functionality

5.1.1.2.1 Allow volunteers to act as citizens per 5.1.1.1

5.1.1.2.2 Allow volunteers to "offer to help" with volunteer needs shown on campaign pages by choosing one or more skills/resources needed and providing contact information to share with organization admin (Name, Email, Phone, Additional Information)

5.1.1.2.3 Volunteer contact information is stored along with the skills/resources needed they selected within the system and a link is provided via email to campaign organization owner and recorded in system

5.1.1.2.4 Allow volunteers to register on the site with social media/third party logins (twitter, facebook, linkedin, google, Microsoft account) to stay connected to the campaign (and with later functionality and/or mobile app be able to receive task assignment 'missions' and record delivery data)

#### 5.1.1.3 Organization Functionality

5.1.1.3.1 Allow organization 'admins' to register on the web application with social media/third party logins (twitter, facebook, linkedin, google, microsoft account) (and register with name, email, organization and organization contact info including name, email, phone, website) [later functionality will allow initial admin to grant other users 'admin' or other perms for organization]

5.1.1.3.2 Allow organization 'admins' to register with username and password on AllReady site as well

5.1.1.3.3 Allow organization 'admins' to register with Office365/Azure AD authenticated logins as well

5.1.1.3.4 Organization admins can create campaigns with landing page content

including but not limited to: Name, Logo/Image, Description [Rich Text Multi Paragraph with images etc.], Keywords, Geography Coverage (zipcode list, city list, state list), Campaign contact info (allow copy from organization but this may be different per campaign), links to campaign web pages, impact target {'x' 'things' by 'y' date} and list of interim goals in either format of {'z' 'things' per 'week/month/quarter'} or {'z' 'things' by 'y' date, 'z1' 'things by 'y1' date, …} or {'textual goal'} the last format of which will not be trackable by the system and will be marked as completed by the admin

5.1.1.3.5 Organization admins can list multiple volunteer skills/resources needed

including optional date ranges and locations {'skill/resource A' needed [from 'x' to 'y' date range] [in 'z' location scope]}.  This listing will allow volunteers to later connect to the organization and offer to volunteer in delivery of the campaign as well as potentially be a data feed consumed by other third party apps that display volunteer needs.

5.1.1.3.6 Organization admins can save campaign info revisions, update them, preview before publishing, publish and publish updates over time  [later functionality (or in V1 if possible) would be to allow citizens to 'report campaigns as spam/inappropriate' and then allow admins to take them down and/or ban org admins]

5.1.1.3.7 Organization admins can provide impact updates against the impact target and interim goals

by updating {'x' 'things' delivered by 'y' date} as well as checking off interim goals achieved per 5.1.1.3.2

#### 5.1.1.4 AllReady Admin Functionality

5.1.1.4.1 Allow AllReady admins to unpublish campaigns published by organization admins [later functionality or in V1 if possible would be to ban org admin accounts]

#### 5.1.2 Volunteer Mobile Application

#### 5.1.2.1 Volunteer Functionality

5.1.2.1.1 Allow volunteer to see tasks assigned to them (authenticated and filtered by social media/third party login or login – thus will only work for those registered) [workflow to be enhanced in > v1]

5.1.2.1.2 Allow volunteer to accept/reject task assignment

5.1.2.1.3 Allow volunteer to mark task completed or 'unable to be completed' with reason/additional text information

#### 5.1.2.2 Organization Admin Functionality

5.1.2.2.1 Allow org admin to create a task and assign to one or more volunteers [later functionality to allow variance by volunteer aka these 5 addresses to be completed by 5 volunteers; have multiple volunteers asked and the first to accept gets it; have task matched by skill in system and/or use machine learning to predict best volunteer etc.]

5.1.2.2.2 Allow org admin to view status (accepted, rejected, completed, incomplete) and reassign tasks to new volunteers.

5.1.2.2.3 Communicate to volunteers via email and txt when assignment requests are made with link to website (which ideally provides link to launch/install app) to interact with request

5.1.2.2.4 [Later functionality will greatly enhance this workflow and reporting and the like]

#### 5.1.3 Operations Staff Application

[All functionality TBD]

#### 5.1.4 Community Member Feedback Application

[All functionality TBD]

## 6 "GO LIVE" V1.1 MINIMAL VIABLE PRODUCT SCOPE

### 6.1 "GO LIVE V1.1" MVP

As documented in this blog post and this standup video, we have an impactful next step opportunity to leverage allReady as the backbone of potentially over 1,250 smoke detector installs in support the June 11th preparedness event for the Washington State/Seattle division of the American Red Cross.  In summary there are 6 major functionality areas we need to focus on (in priority order) to deliver the functionality needed to make this milestone which are listed below.  Of important note is that we are maturing to the stage where we need not only deliver functionality & performance but also strong user experience and ease of use to ensure that the many users this system will have in June have a productive, efficient and pleasant experience using the app to deliver impact to their community.  Therefore, I have highlighted both functional needs as well as user experience needs below.

#### 6.1.1 Featured Campaign and External Link Capability

#### 6.1.1.1 Feature single campaign on front page of app – P1

Feature an upcoming/current campaign with its picture, summary description and link to campaign page on the home page.  For this release we can just select first campaign that fits the criteria as we will have only one campaign 'in production' and for future we will likely add filters, possibly site admin tagging for highlight and featuring more than one campaign.

#### 6.1.1.2 Remove/hide search functionality – P1

Simply don't delete functionality but remove the search box and resulting 'stuff' on home page. This will also 'pause' work on several active issues with search for this release.

#### 6.1.1.3 Display link to list of campaigns on home page – P2

Below the featured campaign provide a link to the campaign list page which shows campaigns that are current/upcoming.  Remove map link and add in logo/image of campaign on home page

#### 6.1.1.4 Add external link and highlight it on campaign display page – P1

Update campaign display page to display and highlight an external link for the campaign to link to an outside page that displays information about the campaign and/or allows folks to request the services of the campaign.  Also verify that the image/logo and rich content description is displayed as well.

#### 6.1.1.5 Enhance UX of campaign display and interaction page – P2

Details TBD but looking to enhance campaign display page to move from 'developer UI' to 'productive/efficient user-focused UX'

#### 6.1.2 Register Volunteers for Itinerary Type Campaigns

#### 6.1.2.1 Entities have new names from user experience discussions – P2

Campaigns are Campaigns, *Activities are Events*, Tasks are Tasks, *Deployments are Itineraries*

These changes need to be reflected in the UI and secondarily in the system/database for consistency & code clarity (and yes we know we are talking entities named Events and Tasks in C# :smile_cat:)

#### 6.1.2.2 Entity Relationships and definitions have (possibly) shifted a bit – P1

- [ ] Campaigns are made up of one or more Events

- [ ] Events are made up of one or more Tasks & Events can span more than one day but a Task cannot

- [ ] A Task is a unique combination of skills & time – therefore a task designates volunteers needed for all of a single timeframe with all of requested skills & further if you need to have different skillsets OR different timeframes you need more than one task

- [ ] Volunteers are assigned to a Task either by volunteering directly or by assignment by an Organizer

- [ ] An Itinerary is a collection of volunteers called a 'team' (from more than one Task in the same event that have the same timeframes but different skillsets) and a collection of requests (from the same campaign the Tasks for volunteers are from) to which they are assigned to deliver

#### 6.1.2.3 Itinerary Campaigns are the new name/functionality for Deployment Managed Tasks – P1

How do they work (changes indicated by * at start)

- [ ] Tasks are still shown to volunteers

- [ ] Volunteers volunteer for Tasks – example: "'Tony' volunteers for 'Install Driver' on Tuesday 8-4"

- [ ] *Volunteers can be assigned to tasks by Organizers (which requires acceptance by volunteer similar to reassignment below)

- [ ] Admins can re-assign tasks to volunteers

- [ ] Re-assignment communication is only sent to Volunteers when it results in change outside of assigned timeframe and they can accept/reject task assignment

- [ ] *Reminder sent ahead of Task with option to respond as 'no longer able to volunteer'

- [ ] *Volunteers from multiple Tasks (in same Event with same timeframe) are collected into an Itinerary along with a set of requests (in same campaign) [[Note this replaces everything referring to Deployments]]

- [ ]  *A Request can be in a single Itinerary only - unless it is marked incomplete from a previous Itinerary in which case it can be recollected into a new Itinerary  (e.g. 1313 Mockingbird Lane install set for Tuesday, on Tuesday it is marked incomplete because no one was home so now it is in the Itinerary for next Thursday installs)

- [ ] *A Volunteer on a Task can be in only one Itinerary (e.g. Tony volunteers as driver on Tuesday so can only be collected into one itinerary – now of course they can have separately signed up for multiple Tasks across multiple days and be in more than one itinerary that way (e.g. Tony volunteered to drive Tuesday, Thursday and next Friday))

#### 6.1.2.4 Events can be duplicated easily to allow for campaigns that consist of repetitive tasks

Effectively this is a UX where I can take an Event and 'copy/paste or duplicate' it changing the start date and the dates of the Event and underlying Tasks are updated accordingly).  All other data remains the same in the duplicate but any volunteer assignments do not travel with the dupliacte

e.g. Event ABC is from 6/1/2016 to 6/2/2016 and has Task 1 on 6/1, Task 2 on 6/1 and Task 3 on 6/2.  When it is duplicated to 6/8/2106 the new Event is from 6/8 to 6/9 and the Tasks are now #1 on 6/8, #2 on 6/8 and #3 on 6/9)

#### 6.1.3 Data Integration with getasmokealarm.org (pull data)

#### 6.1.3.1 Implement a webjob/service to pull incremental net new requests from getasmokealarm.org

This is likely to be 'hardcoded' to this one source now for expediency and as we get more sources we will need to abstract an approach.  The API to pull is being defined by the getasmokealarm.org folks so we will pull them into this issue to discuss how to work together.  The requests should be incrementally added to the database of Requests and be associated with the proper campaign.

#### 6.1.3.2 Maintain database of Requests

Request schema is TBD but will consist of contact information, address, service requested, notes and status and be related as a sub-entity list under a campaign.  Initially we will need to work with the getasmokealarm.org folks to see what they capture, how we would model it and what it needs to look like for our needs.

#### 6.1.3.3 Update statuses as part of delivery of service for requests as per functionality in 6.1.4

As requests are scheduled, completed, or marked incomplete track that status in the request database

 Allow viewing of status by requestor (lowest priority)

With getasmokealarm.org see what contact info we have (email?) to provide a link to view status without revealing private/identifying information so the requestor can check status of requests over time

#### 6.1.4 Construct and Schedule Itineraries for Requests from Volunteers

Asdf

#### 6.1.5 Campaign Progress Dashboard

Asdf map display, installs per timeframe, top volunteers, etc.

#### 6.1.6 Support Rally Type Events for Registration and Signup (lowest priority)

Asdf

