module Utils

let TimestampName name =
    let now = System.DateTime.Now
    sprintf "%s %d.%d.%d.%d.%d" name now.Year now.Month now.Day now.Hour now.Minute

let GetScenarioTestName name = 
    sprintf "[ST] %s" (TimestampName name)