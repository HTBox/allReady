import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { EventsPage } from "./events";
import { EventCardComponent } from "../../components/event-card/event-card";
import { ElasticHeaderModule } from "ionic2-elastic-header/dist";

@NgModule({
  declarations: [
    EventsPage,
    EventCardComponent      
  ],
  imports: [
    IonicPageModule.forChild(EventsPage),
    ElasticHeaderModule
  ],
})
export class EventPageModule {}
