import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { LogoutPage } from './logout';
import { ElasticHeaderModule } from 'ionic2-elastic-header/dist';

@NgModule({
  declarations: [
    LogoutPage,
  ],
  imports: [
    IonicPageModule.forChild(LogoutPage),
    ElasticHeaderModule    
  ],
})
export class LogoutPageModule {}
