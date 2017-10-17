import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { AllReadyEvent } from '../../pages/events/events';
import { SettingsProvider } from '../settings/settings';

@Injectable()
export class SignupProvider {
  constructor(private http: Http, private applicationSettings: SettingsProvider) {
  }

  public signup(event: AllReadyEvent): any {
    var url = this.applicationSettings.EventApiUrl +  "/" + event.Id + "/signup";

    // TODO: Return Observable instead of Promise for code modernization
    var signupRequest = this.http
      .get(url, {})
      .map(response => this.checkForSuccess(
        response.ok, 
        response.status, 
        response.statusText, 
        response.url, 
        response.text()))
      .toPromise();

    return signupRequest;
  }

  private checkForSuccess(ok: boolean, status: number, statusText: string, url: string, text: string): any {
    console.log(ok); // true
    console.log(status); // 200
    console.log(statusText); // ok
    console.log('url' + url); // http://allready-d.azurewebsites.net/api/event/15/signup
    console.log('text:' + text);
  }

}
