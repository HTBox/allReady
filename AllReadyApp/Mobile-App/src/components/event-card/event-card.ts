import { Component, Input } from '@angular/core';
import { AllReadyEvent } from "../../pages/events/events";
import { NavController } from "ionic-angular";
import { EventDetailsPage } from "../../pages/eventdetails/eventdetails";
import { DateFilterPipe } from "../../pipes/datefilter/datefilter";

@Component({
  selector: 'event-card',
  templateUrl: 'event-card.html'
})

export class EventCardComponent {

  @Input('event') event: AllReadyEvent;  
  private formattedStartDateTime: string;
  
  constructor(public navCtrl: NavController, public dateFilterPipe: DateFilterPipe) {
    this.dateFilterPipe = new DateFilterPipe();    
  }
  
  ngOnChanges() {    
    this.formattedStartDateTime = 
      this.dateFilterPipe.transform(this.event.StartDateTime.toString(), null);    
  }

  showEventDetails(event: AllReadyEvent) {
    this.navCtrl.push(EventDetailsPage, { event: event });
  }

}
