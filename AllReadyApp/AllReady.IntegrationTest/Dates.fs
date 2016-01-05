module Dates

open System

let Today _ =
    sprintf "%d/%d/%d" DateTime.Now.Month DateTime.Now.Day DateTime.Now.Year

let Tomorrow _ =
    let tomorrowsDate = DateTime.Today.AddDays(1.0)
    sprintf "%d/%d/%d" tomorrowsDate.Month tomorrowsDate.Day tomorrowsDate.Year

let OneWeekFromToday _ =
    let targetDate = DateTime.Today.AddDays(7.0)
    sprintf "%d/%d/%d" targetDate.Month targetDate.Day targetDate.Year



