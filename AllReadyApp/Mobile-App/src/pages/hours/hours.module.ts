import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { HoursPage } from "./hours";
import { ElasticHeaderModule } from 'ionic2-elastic-header/dist';

@NgModule({
  declarations: [
    HoursPage,
  ],
  imports: [
    IonicPageModule.forChild(HoursPage),
    ElasticHeaderModule    
  ],
})
export class HoursPageModule {}
