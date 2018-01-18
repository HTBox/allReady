import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';
import { EventProvider } from "../../providers/event/event";

@IonicPage()
@Component({
  selector: 'page-events',
  templateUrl: 'events.html',
})
export class EventsPage {

    events: AllReadyEvent[] = new Array<AllReadyEvent>();
openEvents: AllReadyEvent[] = new Array<AllReadyEvent>();

    constructor(
        public navCtrl: NavController,
        public navParams: NavParams,
        public eventService: EventProvider
        ) {
  }

  ionViewDidLoad() {      
      
      this.getEvents();
  }

  getEvents() {
      this.eventService.getActiveEvents().subscribe(response => {          
          this.events = response;
          this.openEvents = this.events;
      });   
  }

}

export class Skill {
    Id: number;
    Name: string;
    Description: string;
    HierarchicalName: string;
}

export class Task {
    AcceptedVolunteerCount: number;
    AmountOfVolunteersNeeded: number;
    Name: string;
    IsAllowSignups: boolean;
    IsClosed: boolean;
    VolunteersRequiredText: string;
    EventId: number;
    Id: number;
}

export class Location {
    Address1: string;
    Address2: string;
    City: string;
    State: string;
    PostalCode: string;
    Country: string;
}

export class AllReadyEvent {
    Id: number;
    OrganizationId: number;
    OrganizationName: string;
    CampaignId: number;
    CampaignName: string;
    Title: string;
    EventType: number;
    Description: string;
    ImageUrl: string;
    TimeZoneId: string;
    StartDateTime: Date;
    EndDateTime: Date;
    Location: Location;
    UserId: string;
    IsClosed: boolean;
    HasPrivacyPolicy: boolean;
    IsLimitVolunteers: boolean;
    IsAllowWaitList: boolean;
    Headline: string;    
    Tasks: Array<Task>;
}
