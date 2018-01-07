import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';

@IonicPage()
@Component({
  selector: 'page-hours',
  templateUrl: 'hours.html',
})
export class HoursPage {

  started: boolean = false;
  stopped: boolean = true;

  constructor(public navCtrl: NavController, public navParams: NavParams) {
  }

  ionViewDidLoad() {

  }

  startstop() {

    this.started = !this.started;
    this.stopped = !this.stopped;

    var date = new Date();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    var day = date.getUTCDate();
    var hour = date.getHours();
    var mins = date.getMinutes();
    var time = `${month}/${day}/${year} ${hour}:${mins}`;

    var msg = `Time ${this.started ? 'in' : 'out'} ${time}`;

    document.getElementById('startstops').innerHTML = "<div class='time'>" + msg + "</div>" + document.getElementById('startstops').innerHTML;
  }
}
