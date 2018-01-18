import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { SettingsProvider } from "../settings/settings";
import { AllReadyEvent } from "../../pages/events/events";
import { Observable } from "rxjs/Observable";

@Injectable()
export class EventProvider {

  constructor(private http: Http, private applicationSettings: SettingsProvider) {

  }

  public getEvents(): Observable<Array<AllReadyEvent>> {
    return this.http
      .get(this.applicationSettings.EventApiUrl)
      .map(res => res.json());
  };

  public getActiveEvents(): Observable<Array<AllReadyEvent>> {
    return this.http
      .get(this.applicationSettings.EventApiUrl)
      .map(res => this.FilterActive(res.json()));
  };

  private FilterActive(allEvents: AllReadyEvent[]): AllReadyEvent[] {
    var today = new Date();
    return allEvents.filter(i =>
      i.IsClosed == false &&
      new Date(i.EndDateTime) >= today);
  }
}