import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';
import { AllReadyEvent, Task } from "../events/events";
import { DateFilterPipe } from "../../pipes/datefilter/datefilter";
import { CheckinProvider } from '../../providers/checkin/checkin';

@IonicPage()
@Component({
  selector: 'page-eventdetails',
  templateUrl: 'eventdetails.html'
})
export class EventDetailsPage {

  event: AllReadyEvent;
  openTasks: Array<Task>;
  state: number = 0;

  public formattedStartDateTime: string;
  public formattedEndDateTime: string;

  public errorMessage: string = "";
  public genericErrorMessage: string = "There was a problem checking you in to the event. Please make sure you have internet access and try again.";

  constructor(
    public navCtrl: NavController,
    public navParams: NavParams,
    public dateFilterPipe: DateFilterPipe,
    public checkinService: CheckinProvider) {
    
      this.event = navParams.get('event');
      this.dateFilterPipe = new DateFilterPipe();
      
      this.openTasks = this.event.Tasks
      .filter(task => task.IsAllowSignups == true)
      .filter(task => task.IsClosed == false);
  }

  ionViewDidLoad() {
    this.formattedStartDateTime =
      this.dateFilterPipe.transform(this.event.StartDateTime.toString(), null);

    this.formattedEndDateTime =
      this.dateFilterPipe.transform(this.event.EndDateTime.toString(), null);
  }

  checkin() {
    this.checkinService.checkinEvent(this.event)
      .then(success => {
        this.state = 1;
      })
      .catch(fail => {
        this.state = 2;
        this.errorMessage = "Unable to check you in due to an error.";
      })
  };

}