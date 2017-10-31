import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { EventDetailsPage } from "./eventdetails";
import { ElasticHeaderModule } from "ionic2-elastic-header/dist";
import { TaskComponent } from "../../components/task/task";

@NgModule({
  declarations: [
    EventDetailsPage,    
    TaskComponent 
  ],
  imports: [
    IonicPageModule.forChild(EventDetailsPage),
    ElasticHeaderModule    
  ],
})
export class EventDetailsPageModule {}
