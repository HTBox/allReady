open canopy
open runner
open System

Threading.Thread.CurrentThread.CurrentCulture <- Globalization.CultureInfo "en-US"

Console.WriteLine("Launching Scenario Tests")

CreatingOrgsAndCampaigns.All Constants.UrlLocalHost
CreatingActivitiesForCampaigns.All Constants.UrlLocalHost

//Chrome Driver: https://github.com/SeleniumHQ/selenium/wiki/ChromeDriver
canopy.configuration.chromeDir <- System.AppDomain.CurrentDomain.BaseDirectory
let chromeOptions = OpenQA.Selenium.Chrome.ChromeOptions()
chromeOptions.AddArgument("--disable-extensions")
start <| ChromeWithOptions chromeOptions
pin Left
resize (1024, 768)
Console.WriteLine("Launched Chrome")

////Firefox has issues with Selnium 2.5.x.  see https://github.com/lefthandedgoat/canopy/issues/288.  You have to installed version 48.
//start firefox 
//pin Right
//resize(1024, 768)
//Console.WriteLine("Launched Firefox")

//Test don't work in IE right now.  Many times IE adds a space to the end of the element text
// IE Driver: https://github.com/SeleniumHQ/selenium/wiki/InternetExplorerDriver
//canopy.configuration.ieDir <- System.AppDomain.CurrentDomain.BaseDirectory
//start ie
//pin Left
//resize(1024, 768)
//Console.WriteLine("Launched IE")

//Test don't work in Edge right now.  Many times Edge adds a space to the end of the element text
// Edge Driver: https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/
//canopy.configuration.edgeDir <- System.AppDomain.CurrentDomain.BaseDirectory
//
//start edgeBETA
//pin Right
//resize(1024, 768)
//Console.WriteLine("Launched EDGE")


//run all tests
run()

System.Environment.ExitCode <- runner.failedCount
Console.WriteLine(String.Format("{0} passed, {1} failed", runner.passedCount, runner.failedCount))

// only turn on when running directly from Visual Studio so that you can look at any errors.
//Console.WriteLine("Press Enter to Exit")
//let line = Console.ReadLine()

quit()
