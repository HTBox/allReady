module Actions

open canopy
open runner
open System

let GoToHomePage baseUrl = 
    url baseUrl

let Login userName password baseUrl =
    GoToHomePage baseUrl
    click ".log-in"
    "input#Email" << userName
    "input#Password" << password
    click "#login-submit"
    "a[title='Manage']" == ("Hello " + userName + "!")
