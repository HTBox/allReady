import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import { SettingsProvider } from "../settings/settings";
import { LoginRequest } from "../../models/loginrequest";
import { User } from "../../models/user";

@Injectable()
export class AuthProvider {

  public isAuthenticated = false;
  public AuthenticatedUser: User = null;  

  constructor(
    public http: Http, 
    private applicationSettings: SettingsProvider
  ) {
      
  }

  public login(loginRequest: LoginRequest): Promise<boolean> {
   
    var call = this.http.post(this.applicationSettings.LoginApiUrl, loginRequest);
  
    var authResultPromise = call.map(content => {
        this.isAuthenticated = true;
        this.AuthenticatedUser = new User();
        this.AuthenticatedUser.Username = loginRequest.Email;        
        return true;
      
    })
    .toPromise()
    .catch(failure => {
      this.isAuthenticated = false;
      return false;
    });

    return authResultPromise;
  };

}
