import { BrowserModule } from '@angular/platform-browser';
import { ErrorHandler, NgModule } from '@angular/core';
import { IonicApp, IonicErrorHandler, IonicModule } from 'ionic-angular';
import { SplashScreen } from '@ionic-native/splash-screen';
import { StatusBar } from '@ionic-native/status-bar';
import { HttpModule } from '@angular/http';
import { MyApp } from './app.component';
import { CacheProvider } from '../providers/cache/cache';
import { EventProvider } from '../providers/event/event';
import { CheckinProvider } from '../providers/checkin/checkin';
import { AuthProvider } from '../providers/auth/auth';
import { SettingsProvider } from '../providers/settings/settings';
import { TimefilterPipe } from '../pipes/timefilter/timefilter';
import { EventsPage } from "../pages/events/events";
import { LoginPage } from "../pages/login/login";
import { HoursPage } from "../pages/hours/hours";
import { SettingsPage } from "../pages/settings/settings";
import { ElasticHeaderModule } from "ionic2-elastic-header/dist";
import { EventDetailsPage } from "../pages/eventdetails/eventdetails";
import { InAppBrowser } from "@ionic-native/in-app-browser";
import { SideMenuProvider } from '../providers/sidemenu/sidemenu';
import { EventPageModule } from "../pages/events/events.module";
import { EventDetailsPageModule } from "../pages/eventdetails/eventdetails.module";
import { LoginPageModule } from "../pages/login/login.module";
import { HoursPageModule } from "../pages/hours/hours.module";
import { SettingsPageModule } from "../pages/settings/settings.module";
import { DateFilterPipe } from "../pipes/datefilter/datefilter";
import { SignupProvider } from '../providers/signup/signup';
import { LogoutPageModule } from '../pages/logout/logout.module';
import { LogoutPage } from '../pages/logout/logout';

@NgModule({
  declarations: [
    MyApp,    
    TimefilterPipe,
    DateFilterPipe
  ],
  imports: [
    BrowserModule,
    HttpModule,
    IonicModule.forRoot(MyApp),    
    ElasticHeaderModule,
    EventPageModule,
    EventDetailsPageModule,
    LoginPageModule,
    HoursPageModule,
    SettingsPageModule,
    LogoutPageModule
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    MyApp,
    EventsPage,
    EventDetailsPage,
    LoginPage,
    HoursPage,
    SettingsPage,
    LogoutPage
  ],
  providers: [
    StatusBar,
    SplashScreen,
    { provide: ErrorHandler, useClass: IonicErrorHandler },
    CacheProvider,
    EventProvider,
    CheckinProvider,
    AuthProvider,
    SettingsProvider,
    InAppBrowser,
    SideMenuProvider,
    DateFilterPipe,
    SignupProvider
  ]
})
export class AppModule { }
