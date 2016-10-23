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
    let title = read "a[title='Manage']" 
    let expectedTitle = ("Hello " + userName)
    //title == ("Hello " + userName)
    contains expectedTitle title
