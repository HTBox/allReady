import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { SideMenuItem } from "../../models/sidemenuitem";
import { LoginPage } from "../../pages/login/login";
import { EventsPage } from "../../pages/events/events";
import { HoursPage } from "../../pages/hours/hours";
import { SettingsPage } from "../../pages/settings/settings";
import { AuthProvider } from "../auth/auth";
import { LogoutPage } from '../../pages/logout/logout';

@Injectable()
export class SideMenuProvider {

  constructor(public http: Http, public auth: AuthProvider) {
    
  }

  public GetSideMenuItems(): Array<SideMenuItem> {
    
var isAuthenticated = (this.auth && !this.auth.isAuthenticated);

            var sideMenuItems: Array<SideMenuItem> = [
                { title: 'Login', component: LoginPage, active: false, icon: 'person-add', show: isAuthenticated  },                
                { title: 'Events', component: EventsPage, active: false, icon: 'calendar', show: true },                            
                { title: 'Hours', component: HoursPage, active: false, icon: 'alarm', show: true },
                { title: 'Settings', component: SettingsPage, active: false, icon: 'settings', show: true },
                { title: 'Log Out', component: LogoutPage, active: false, icon: 'log-out', show: !isAuthenticated  },
            ];
    
            return sideMenuItems;
        }

}
