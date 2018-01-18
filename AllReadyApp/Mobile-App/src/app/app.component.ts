import { Component,ViewChild } from '@angular/core';
import { Platform, Nav } from 'ionic-angular';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { EventsPage } from "../pages/events/events";
import { SideMenuItem } from "../models/sidemenuitem";
import { AuthProvider } from "../providers/auth/auth";
import { SideMenuProvider } from "../providers/sidemenu/sidemenu";

@Component({
  templateUrl: 'app.html'
})
export class MyApp {
  @ViewChild(Nav) nav: Nav;
  rootPage: any = EventsPage;
  sideMenuPages: Array<SideMenuItem>;
  pictureUrl: string = 'assets/img/avatars/girl-avatar.png';

  constructor(platform: Platform, statusBar: StatusBar, splashScreen: SplashScreen, auth: AuthProvider, public sideMenu: SideMenuProvider) {
    platform.ready().then(() => {
      // Okay, so the platform is ready and our plugins are available.
      // Here you can do any higher level native things you might need.
      statusBar.styleDefault();
      splashScreen.hide();
    });
  }

  openPage(page) {
      this.nav.setRoot(page.component);     
  }
}
