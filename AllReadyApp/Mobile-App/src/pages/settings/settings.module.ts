import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { SettingsPage } from "./settings";
import { ElasticHeaderModule } from 'ionic2-elastic-header/dist';

@NgModule({
  declarations: [
    SettingsPage,
  ],
  imports: [
    IonicPageModule.forChild(SettingsPage),
    ElasticHeaderModule    
  ],
})
export class SettingsPageModule {}
