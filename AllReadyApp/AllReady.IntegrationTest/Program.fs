open canopy.classic
open canopy.runner.classic
open canopy.types
open System

//Threading.Thread.CurrentThread.CurrentCulture <- Globalization.CultureInfo "en-US"

Console.WriteLine("Launching Scenario Tests")

CreatingOrgsAndCampaigns.All Constants.UrlLocalHost
CreatingActivitiesForCampaigns.All Constants.UrlLocalHost

// want to use the chromedriver in current directory to ensure match with current version of Chrome
canopy.configuration.chromeDir <- System.AppDomain.CurrentDomain.BaseDirectory

//let chromeOptions = OpenQA.Selenium.Chrome.ChromeOptions()
//chromeOptions.AddArgument("--disable-extensions")
//start <| ChromeWithOptions chromeOptions
//pin Left
//resize (1024, 768)

// start chrome browser, pin to left of screen and resize
start chrome
pin Left
resize (1024,768)

Console.WriteLine("Launched Chrome")

//run all tests
run()

System.Environment.ExitCode <- failedCount
Console.WriteLine(String.Format("{0} passed, {1} failed", passedCount, failedCount))

// only turn on when running directly from Visual Studio so that you can look at any errors.
//Console.WriteLine("Press Enter to Exit")
//let line = Console.ReadLine()

quit()
