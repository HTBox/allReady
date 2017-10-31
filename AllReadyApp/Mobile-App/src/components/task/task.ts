import { Component, Input } from '@angular/core';
import { Task } from "../../pages/events/events";
import { InAppBrowser } from '@ionic-native/in-app-browser';
import { SettingsProvider } from "../../providers/settings/settings";

@Component({
  selector: 'task',
  templateUrl: 'task.html'
})
export class TaskComponent {

@Input('task') task: Task;

  constructor(private iab: InAppBrowser, private settingsProvider: SettingsProvider) {
    
  }

  public volunteer() {    
    let url = this.settingsProvider.BaseUrl + "/Event/" + this.task.EventId;
    this.iab.create(url, '_self');
  }

}
