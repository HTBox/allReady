[33mcommit b7e70febf15a7a6e51631d4b70917aad63731a04[m
Merge: bb16a54 a8371dc
Author: Tony Surma <tony@surmafamily.com>
Date:   Wed Nov 23 11:33:53 2016 -0800

    Merge pull request #1555 from stevejgordon/CodeAThons
    
    Adding code-a-thons to readme

[33mcommit a8371dc7a52dd4804884851b4bf93fbce4501d4a[m
Author: Steve Gordon <sgordon@hotmail.co.uk>
Date:   Wed Nov 23 18:17:14 2016 +0000

    Formatting fixup

[33mcommit 04f75b6245505fb373431959ff7966475940ae6f[m
Author: Steve Gordon <sgordon@hotmail.co.uk>
Date:   Wed Nov 23 18:15:26 2016 +0000

    Adding code-a-thons to readme

[33mcommit bb16a54f8343f4c20b332a593c0f3d30df48b2f8[m
Merge: 9655809 4c5e665
Author: Tony Surma <tony@surmafamily.com>
Date:   Wed Nov 23 07:41:21 2016 -0800

    Merge pull request #1553 from HamidMosalla/issue-branch-1435
    
    Implement skipped tests in TaskApiControllerTests #1435

[33mcommit 4c5e66532a5078b3d664813f17d42112cfec273c[m
Author: Hamid Mosalla <Xellarix@gmail.com>
Date:   Wed Nov 23 18:39:30 2016 +0330

    remove InmemoryContext and mock userManager instead

[33mcommit 83148123c223a0c4ef9df5282e257d1086e7b475[m
Author: Hamid Mosalla <Xellarix@gmail.com>
Date:   Tue Nov 22 23:29:29 2016 +0330

    Implement skipped tests in TaskApiControllerTests #1435

[33mcommit 965580966656f9d6943f68a07ef75fc45d947c62[m
Merge: 15aa4d4 1cc597f
Author: Tony Surma <tony@surmafamily.com>
Date:   Tue Nov 22 06:57:26 2016 -0800

    Merge pull request #1552 from junderhill/patch-1
    
    Commit message as command argument

[33mcommit 1cc597f6b98f6f48560982730cd84ecbaa6ab18e[m
Author: Jason Underhill <jay.underhill@gmail.com>
Date:   Tue Nov 22 12:05:34 2016 +0000

    Commit message as command argument
    
    Provide instructions on using the '-m' argument opposed to the vi editor for entering a commit message for #1283

[33mcommit 15aa4d4a53be7533f573d8f002b45c48d9e6dc7d[m
Merge: bf39e05 3b6a6d4
Author: Tony Surma <tony@surmafamily.com>
Date:   Mon Nov 21 19:53:15 2016 -0800

    Merge pull request #1551 from mgmccarthy/Fix1549
    
    Fix #1549

[33mcommit 3b6a6d428a805f0a0562203cae9e5412a35d28f9[m
Author: Michael McCarthy <mike_mccarthy3@yahoo.com>
Date:   Mon Nov 21 15:13:33 2016 -0500

    Add EventId property assignemtn for Itinerary back to EditItineraryCommandHandler

[33mcommit bf39e05a1ed196af0264e9adad52c86d3d4225f9[m
Merge: 0828789 92e6b68
Author: Tony Surma <tony@surmafamily.com>
Date:   Mon Nov 21 07:13:13 2016 -0800

    Merge pull request #1540 from stefan-van-de-griendt-centric/user-manage-cancel
    
    Fixes #1356

[33mcommit 0828789d66f24bc1950191cea1d0013eba20707b[m
Merge: cbbf68f 95c5bcf
Author: Tony Surma <tony@surmafamily.com>
Date:   Mon Nov 21 07:09:49 2016 -0800

    Merge pull request #1547 from mgmccarthy/RemoveIntinerariesPropertyFromRequestBecauseItsNotUsed
    
    Remove Itineraries Property From Request

[33mcommit cbbf68f6cfe71e353fc063e00d01a09798d7a059[m
Merge: bead6c5 d3e041b
Author: Tony Surma <tony@surmafamily.com>
Date:   Mon Nov 21 07:09:37 2016 -0800

    Merge pull request #1546 from mgmccarthy/RenameRequestStatusChangeCommandAndHandler
    
    Rename RequetsStatusChangeCommand/Handler and unit tests

[33mcommit bead6c550ee8e24a459c0227ddbac07159a00838[m
Merge: 35974dc 7535952
Author: Tony Surma <tony@surmafamily.com>
Date:   Mon Nov 21 07:08:34 2016 -0800

    Merge pull request #1544 from HamidMosalla/issue-branch-1431
    
    Implement skipped tests in AddTeamMemberCommandHandlerShould #1431

[33mcommit 95c5bcfd54e2e5cd8b575614ff8f8318430e7e89[m
Author: Michael McCarthy <mike_mccarthy3@yahoo.com>
Date:   Sun Nov 20 21:19:38 2016 -0500

    RemoveItinerariesPropertyFromRequest

[33mcommit d3e041b1338ec0565567f933edfce35d4b21442d[m
Author: Michael McCarthy <mike_mccarthy3@yahoo.com>
Date:   Sun Nov 20 20:43:13 2016 -0500

    Rename RequetsStatusChangeCommand the handler and the unit test to ChangeRequestStatusCommand

[33mcommit 75359527842001de67b6c709f9d08372fbc44b6d[m
Author: Hamid Mosalla <Xellarix@gmail.com>
Date:   Sat Nov 19 21:40:59 2016 +0330

    Implement skipped tests in AddTeamMemberCommandHandlerShould #1431

[33mcommit 92e6b68091354b1b97b95d78c4cc161fbadf6f8a[m
Author: stefan-van-de-griendt-centric <stefan.van.de.griendt@centric.eu>
Date:   Sat Nov 19 19:07:27 2016 +0100

    Changed to proper `nameof` contents
    `nameof` only returns the name of the last entity in the chain
    No, directly the action `Index` is used instead of something that coincidentally has an `Index` property

[33mcommit 4a0c8317a777090794592c4956b26ab63bb05577[m
Author: stefan-van-de-griendt-centric <stefan.van.de.griendt@centric.eu>
Date:   Sat Nov 19 18:44:58 2016 +0100

    Moved from button to anchor
    - Now uses asp-controller and asp-action as navigation
    - Removed previously added `Button` property and related code handling
    
    **Important**
    Note the class `bnt-danger` to add a distinction between the two.
    Perhaps it requires special styling?

[33mcommit 35974dc95582d37772b30e6241cf15524eb0dfa7[m
Author: Tony Surma <tony@surmafamily.com>
Date:   Sat Nov 19 11:33:08 2016 -0600

    fix for deploy commit tag to live site

[33mcommit 122397a97f80656bb56305c1287d3e1088ee1b7f[m
Author: Hamid Mosalla <HamidMosalla@users.noreply.github.com>
Date:   Sat Nov 19 20:38:37 2016 +0330

    Implement skipped tests in DetermineIfATaskIsEditableShould #1434 (#1543)

[33mcommit 1cee961c60336e136a9e6c1406d1c3478c4e829a[m
Author: Tony Surma <tony@surmafamily.com>
Date:   Sat Nov 19 10:38:54 2016 -0600

    fix to find global.json in azure deployment build

[33mcommit 1b0b8d86a03d168ad520e9b82cef4d4635b36c19[m
Author: rkiggen <robby.kiggen@essential-it.be>
Date:   Sat Nov 19 17:17:17 2016 +0100

    Issue branch 1517 (#1536)
    
    * changed validation message for Rule 'End date cannot be out of range of parent event'
    
    * Issue 1517
    changed validation message for Rule - End date cannot be out of range of
    parent event
    
    * Issue 1517
    Changed Task Edit Validation messages to use the g date pattern and
    replace string concatenations by string.format
    
    * Issue 1517
    changed task tests to reflect changed validation messages

[33mcommit 83584177d2881ee7c5d926a6e574dcbedf218572[m
Author: Hamid Mosalla <HamidMosalla@users.noreply.github.com>
Date:   Sat Nov 19 19:46:40 2016 +0330

    Implement skipped tests in EditOrganizationHandlerShould #1433 (#1541)

[33mcommit a6cd173314c2c18606e3ed4132d1ac6a0895e3d1[m
Author: Hamid Mosalla <HamidMosalla@users.noreply.github.com>
Date:   Sat Nov 19 19:44:55 2016 +0330

    Implement skipped tests in EditItineraryCommandHandlerShould #1432 (#1537)

[33mcommit a4e87744770dbacdc2315433cdbfaf59c2f489ad[m
Author: waes <ivanwaesberghe@outlook.com>
Date:   Sat Nov 19 17:14:07 2016 +0100

    added EventViewModelShould #1439 test data (#1539)

[33mcommit 75095a32a8a8fa759b51cb74ff8f6773422fdd88[m
Author: GITS-ONLINE <info@gits-online.nl>
Date:   Sat Nov 19 16:56:43 2016 +0100

    #1513 (#1535)
    
    Fixed!
    Hackaton Eindhoven, together with Bjorn Pirson

[33mcommit 1db998a2b67ef9a700bbdb1908cdd74fb72c2f16[m
Author: stefan-van-de-griendt-centric <stefan.van.de.griendt@centric.eu>
Date:   Sat Nov 19 16:52:30 2016 +0100

    Fixes #1356
    Added cancel button and handling of it

[33mcommit 79bced8899987be26e3951488b2184ac37a423b9[m
Merge: bc2c8dc df5bcd0
Author: Tony Surma <tony@surmafamily.com>
Date:   Sat Nov 19 07:11:36 2016 -0800

    Merge pull request #1475 from pranap/Issue1456
    
    Issue1456

[33mcommit bc2c8dcc30cd84051260dc1febe00b6c9565bb71[m
Author: naebliss <naebliss666@hotmail.com>
Date:   Sat Nov 19 15:27:17 2016 +0100

    #1530: fixed unittest to be property specific and language fixed (#1533)

[33mcommit e1a7d5d2c5a4f786bc070974d26c2d1df0effc78[m
Author: Daniel Schiavini <daniel.schiavini@gmail.com>
Date:   Sat Nov 19 06:21:25 2016 -0800

    Include more data in volunteers task grid #1520 (#1531)

[33mcommit 83184caebf293aa9e8b139cc7dce75127378adac[m
Author: Daniel Schiavini <daniel.schiavini@gmail.com>
Date:   Sat Nov 19 06:18:41 2016 -0800

    Featured campaigns legend #1504 (#1532)

[33mcommit d497d666b8062774b175fdee385b6c17a7aae9b7[m
Author: Barry Luijbregts <b.luijbregts@waardedoorit.nl>
Date:   Sat Nov 19 14:51:31 2016 +0100

    #1521 Changed Phone number into Mobile phone number in a number of places and added a static text saying (Standard text message rates apply) (#1529)

[33mcommit 6dab914bcf7626e670a8542852f46429da1ca98a[m
Merge: 0e143b9 7543dc3
Author: Tony Surma <tony@surmafamily.com>
Date:   Fri Nov 18 15:32:18 2016 -0800

    Merge pull request #1528 from dpaquette/bug/1526
    
    Fix for unit test that failed at certain times of day

[33mcommit 7543dc34b8dc4c016e8f115b351f4f6a1a96c26d[m
Author: David Paquette <dave@davepaquette.com>
Date:   Fri Nov 18 16:09:59 2016 -0700

    Fix for unit test that failed at certain times of day

[33mcommit 0e143b993e65d9f77a95c21714af681db2872484[m
Merge: 3d629e0 a4ad52a
Author: Tony Surma <tony@surmafamily.com>
Date:   Fri Nov 18 15:05:58 2016 -0800

    Merge pull request #1527 from dpaquette/bug/datetimeformats
    
    Missed project when updating to .net 4.6

[33mcommit a4ad52aca5aa53f065c3232ac3f6af5287cbd63b[m
Author: David Paquette <dave@davepaquette.com>
Date:   Fri Nov 18 15:55:22 2016 -0700

    Missed project when updating to .net 4.6

[33mcommit 3d629e05e6b9c7840b0b1f32fd68e89c55b44746[m
Merge: 4e93fc3 e988fb2
Author: Tony Surma <tony@surmafamily.com>
Date:   Fri Nov 18 14:11:35 2016 -0800

    Merge pull request #1494 from mgmccarthy/HangfireJobsUseEventsTimeZoneId
    
    Hangfire jobs use events time zone

[33mcommit 4e93fc3f4aa81d33a94cbc0478bd0cd19c29125e[m
Merge: 90c712b c535bae
Author: Tony Surma <tony@surmafamily.com>
Date:   Fri Nov 18 14:10:28 2016 -0800

    Merge pull request #1524 from dpaquette/bug/datetimeformats
    
    Target .NET 4.6 to fix thread context switch losing CurrentCulture an…

[33mcommit c535baed97d2fc6738938fe5a71820275c8ab18f[m
Author: David Paquette <dave@davepaquette.com>
Date:   Fri Nov 18 09:10:57 2016 -0700

    Target .NET 4.6 to fix thread context switch losing CurrentCulture and CurrentUICulture
    
    As per https://github.com/aspnet/Mvc/issues/4692
    
    Resolves issues #1224 and #1223

[33mcommit 90c712b325fd6fb8c97525be074f242387e7a930[m
Merge: f77a315 f8b6e3d
Author: Tony Surma <tony@surmafamily.com>
Date:   Fri Nov 18 05:10:45 2016 -0800

    Merge pull request #1495 from stevejgordon/#1331
    
    Adding ability to edit itinerary

[33mcommit f8b6e3ddce5d91ba8ca65bbeed7e1111c21c79be[m
Author: Steve Gordon <sgordon@hotmail.co.uk>
Date:   Fri Nov 18 08:13:41 2016 +0000

    Removing ability to edit date for an itinerary

[33mcommit f77a315014626dedfb68c50810a35b5be6719914[m
Merge: 5256f10 6e8a792
Author: Tony Surma <tony@surmafamily.com>
Date:   Thu Nov 17 09:09:44 2016 -0600

    Merge pull request #1492 from tonylambert/issue-branch-1398
    
    Request creation/edit looks up lat/long for address

[33mcommit e988fb2d2ef8301458d7f4773290d9a4316a12e8[m
Author: Michael McCarthy <mike_mccarthy3@yahoo.com>
Date:   Thu Nov 17 09:37:00 2016 -0500

    Updating the unit tests in this commit to use DateTime.Today instead of DateTime.Now to align with how the UI sends us the Itinerary.Date value

[33mcommit 2edfc5c356289c50ae8a84af1e6e7e03a5f4af2c[m
Author: Michael McCarthy <mike_mccarthy3@yahoo.com>
Date:   Thu Nov 17 09:15:40 2016 -0500

    code cleanup

[33mcommit 6e8a7924652e05f82ab5824f739fc3d20cbc77d1[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Wed Nov 16 20:18:06 2016 -0500

    Update lat/long only when address changed
    
    * When editing a request only update the lat/long when the address
      changed instead of always.

[33mcommit 8d730d70e557c129a8cb02cbd55715c857aad9a1[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Wed Nov 16 20:01:36 2016 -0500

    Remove unnecessary using statement

[33mcommit c04302dd5bd49a83cf075e0255c70789e4264319[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Wed Nov 16 19:43:29 2016 -0500

    Remove unused local variables

[33mcommit a992d0aa49ce98821bf05365ad5e4ab39f43d484[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Wed Nov 16 07:56:33 2016 -0500

    Always update lat/long for existing records
    
    * It isn't known whether the address changed when a Request is edited so
      update lat/long from Geocoder on updates.

[33mcommit 015cdcacc9877ac99fdc70d8ac5016c3d342a06c[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Wed Nov 16 07:44:07 2016 -0500

    EditRequestCommandHandler - Verify geocode method
    
    * Verify the Request's lattitude and longitude are set when a request is
      created or updated.

[33mcommit 623865bb2c74b4a1b062e20ab0264795dbdebe8b[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Wed Nov 16 07:25:25 2016 -0500

    EditRequestCommandHandler Update support

[33mcommit 64d371390ebef423310011aa7cc778cb3d058622[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Wed Nov 16 07:06:06 2016 -0500

    Add happy path Request create unit test

[33mcommit 73949898d75be8b8593dbca3dd19d139cd63dd4f[m
Author: Tony Lambert <tonylambert@mgmail.com>
Date:   Tue Nov 15 23:43:50 2016 -0500

    Naive implementation for lat/long

[33mcommit 045ca757b02449d72e12daaf329a86aceff20976[m
Author: Steve Gordon <sgordon@hotmail.co.uk>
Date:   Wed Nov 16 17:56:18 2016 +0000

    Adding unit tests for itinerary controller

[33mcommit 8152dbc2b1a12b4cdb245e8c2bdcfb4b71be9e2b[m
Author: stevejgordon <sgordon@hotmail.co.uk>
Date:   Wed Nov 9 13:14:12 2016 +0000

    Adding handlers and updating controller / view code

[33mcommit 6f9d5557daa46c7117f79f5b9d0c109f2688d8ae[m
Author: Steve Gordon <sgordon@hotmail.co.uk>
Date:   Mon Nov 7 19:24:48 2016 +0000

    Schema change to support storing itinerary start and end addresses

[33mcommit 2c4d18606986bad7a8aded2d974adfecef3cf83b[m
Author: Steve Gordon <sgordon@hotmail.co.uk>
Date:   Mon Nov 7 19:08:26 2016 +0000

    Initial work to add an itinerary edit page
    
    - Adding edit (GET) action
    - Updating view model
    - Adding handler to populate model
    - Update views

[33mcommit 42aceb8a6c8c773a8315c64d2acec1002caf2850[m
Author: Michael McCarthy <mike_mccarthy3@yahoo.com>
Date:   Wed Nov 16 12:20:38 2016 -0500

    Update all Hangfire job classes to convert to Event's TimeZoneId when doing DateTime comparisons or scheduling next Hangfire job in pipeline.

[33mcommit 8371913ac5939b41b87799da0ca56bd95c450016[m
Author: Michael McCarthy <mike_mccarthy3@yahoo.com>
Date:   Tue Nov 15 19:21:44 2016 -0500

    First commit at adding time zone conversion for each scheduled Hangfire job in \Hangfire\Jobs. The solution does not compile right now b/c of broken unit tests
    
    - uses the Request's  Itinerary's Event's TimeZoneId for the time zone conversion
    - changed the check in each job's method that checks for if "now" is equal to the number of days before the job should have been scheudled to fire. Before, it checked for equal to or greater than. I'm not too sure why I did that check. I changed it to have to equal the amount of days. An example is for SendRequestConfirmationMessagesADayBeforeAnItineraryDate, now has to be equal to one day before the requests itinerary's date (converted to the local time using the event's time zone id)
    - renaming incorrectly spelled SetRequstsToUnassignedCommand/Handler
    - TODO:
    - I need to also convert to local time using the event's time zone id when chekcing for 7 days, 1 day, and day of comparisons in each respective job class to make sure I account for the requestor's local time zone
    - none of the tests compile. They'll have to be fixed/updated

[33mcommit 5256f1047ec4619cc6608f8b02192d69732a2123[m
Merge: 2581ecf 5428472
Author: Tony Surma <tony@surmafamily.com>
Date:   Wed Nov 16 09:36:59 2016 -0600

    Merge pull request #1488 from dchristensen/issue-1484
    
    Moved text on 404 screen above image.
