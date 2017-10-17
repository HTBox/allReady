import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { AllReadyEvent } from '../../pages/events/events';
import { SettingsProvider } from '../settings/settings';

@Injectable()
export class CheckinProvider {

  checkForSuccess(res: any): boolean {

    let eventCheckinResponse: Response = res as Response;
    let containsSpecialText: boolean = res._body.includes("You should volunteer too!");

    if (eventCheckinResponse.ok == true
      && eventCheckinResponse.status == 200
      && containsSpecialText) {
        
      return true;
    }

    return false;
  }

  event: AllReadyEvent;

  constructor(private http: Http, private applicationSettings: SettingsProvider) {

  }

  checkinEvent(event: AllReadyEvent): any {
    var checkinUrl = this.applicationSettings.EventApiUrl + "/" + event.Id + "/checkin";
 
    var checkinResult = this.http
      .get(checkinUrl, {})
      .map(res => this.checkForSuccess(res))
      .toPromise();

    return checkinResult;
  }
}
