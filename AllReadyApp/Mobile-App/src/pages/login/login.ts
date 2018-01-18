import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';
import { AuthProvider } from "../../providers/auth/auth";
import { LoginRequest } from "../../models/loginrequest";
import { EventsPage } from "../events/events";

@IonicPage()
@Component({
  selector: 'page-login',
  templateUrl: 'login.html',
})
export class LoginPage {

  Credentials: LoginRequest = new LoginRequest();

  constructor(public navCtrl: NavController,
    public navParams: NavParams,
    private auth: AuthProvider
  ) {
  }

  login() {
    this.auth.login(this.Credentials)
      .then(result => {        
      if (result) {
        this.navCtrl.setRoot(EventsPage);       
      } else {        
        alert("Access Denied");
      }
    })
  };

}
