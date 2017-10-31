import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { LoginPage } from "./login";
import { ElasticHeaderModule } from "ionic2-elastic-header/dist";

@NgModule({
  declarations: [
    LoginPage    
  ],
  imports: [
    IonicPageModule.forChild(LoginPage),
    ElasticHeaderModule
  ],
})
export class LoginPageModule {}
