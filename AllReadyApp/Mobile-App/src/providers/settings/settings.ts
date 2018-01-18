import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';

@Injectable()
export class SettingsProvider {

  constructor() { 
    
  }

  //readonly BaseUrl = 'http://localhost:48408';
  readonly BaseUrl = 'http://allready-d.azurewebsites.net';
  readonly BaseApiUrl = this.BaseUrl + '/api';
  readonly LoginApiUrl = this.BaseApiUrl + "/me/login"
  readonly EventApiUrl = this.BaseApiUrl + '/event';

}