open canopy
open runner
open System

Console.WriteLine("Launching Scenario Tests")

//start an instance of the firefox browser
start firefox
pin Left
resize (1024, 768)

Console.WriteLine("Launched Firefox")

CreatingOrgsAndCampaigns.All Constants.UrlLocalHost
CreatingActivitiesForCampaigns.All Constants.UrlLocalHost


//run all tests
run()

System.Environment.ExitCode <- runner.failedCount


quit()
