import { Component, Input, Inject, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../service/auth.service';
import { Observable, Subscription } from "rxjs";
import { HOST_ATTR } from "@angular/platform-browser/src/dom/dom_renderer";
import { TimerObservable } from "rxjs/observable/TimerObservable";
import { isUndefined } from "util";

@Component({
  selector: 'stopwatch',
  templateUrl: './stopwatch.component.html',
  styleUrls: ['./stopwatch.component.less']
})

export class StopwatchComponent implements OnInit {
  elementTimeSpan: TimeSpanElement;
  @Input() elementId: number;
  @Input() class: string;
  @Input() currentInterval: boolean;
  IsOpen: boolean;
  hours: string;
  minutes: string;
  seconds: string;
  timer: Observable<number>;
  timerSubscription: Subscription;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string
  ) { }

  ngOnInit() {

    this.timer = Observable.timer(1000, 1000);

    this.activatedRoute.params.subscribe(params => {
      var id = params["id"];
      if (id) {

         //In case if there wasn't input initiation
        if (this.elementId != id) this.elementId = id;
        
        this.getElementTimeSpan();
        console.log("Element with id" + this.elementId + " has been got time spent data");
      }
      else {
        console.log("Stopwatch element Id:" + this.elementId);
        this.getElementTimeSpan();
      }
    })
  }

  ngOnDestroy() {
    this.StopTimer()
  }
 

  getElementTimeSpan() {
    var url = this.baseUrl;
    if (this.currentInterval) url += "api/interval/current/" + this.elementId;
      else url += "api/interval/element/" + this.elementId;
    this.http.get<TimeSpanElement>(url).subscribe(result => {
      this.elementTimeSpan = result;

      //if swithed to other NodeElement
      this.StopTimer();

      if (this.elementTimeSpan.IsOpenIntervalId != 0) {
        this.StartTimer();
      }
      else {
        this.hours = "" + this.elementTimeSpan.Hours;
        this.minutes = this.elementTimeSpan.Minutes < 10 ? "0" + this.elementTimeSpan.Minutes : "" + this.elementTimeSpan.Minutes;
        this.seconds = this.elementTimeSpan.Seconds < 10 ? "0" + this.elementTimeSpan.Seconds : "" + this.elementTimeSpan.Seconds;
      }
    }, error => console.error(error));
  }

  timerUpdate() {
    this.elementTimeSpan.Seconds++;
  }

  StartTimer() {

    //Delete previose subscription if exist
    this.StopTimer();

    var sec = this.elementTimeSpan.Seconds;
    var min = this.elementTimeSpan.Minutes;
    var hour = this.elementTimeSpan.Hours;

    //Properties that used in stopwatch.component.html
    this.seconds = sec < 10 ? "0" + sec : "" + sec;
    this.minutes = min < 10 ? "0" + min : "" + min;
    this.hours = "" + hour;

    //if timer still work on this element
    if (this.timerSubscription) {
      if (this.timerSubscription.closed == false) return;
    }

    this.timerSubscription = this.timer.subscribe(() => {

      //Update timer every second
      sec++;
      if (sec == 60) {
        min++;
        if (min == 60) {
          hour++;
          min = 0;
          this.elementTimeSpan.Hours = hour;
        }
        this.elementTimeSpan.Minutes = min;
        sec = 0;
      }
      this.elementTimeSpan.Seconds = sec;

      this.seconds = sec < 10 ? "0" + sec : "" + sec;
      this.minutes = min < 10 ? "0" + min : "" + min;
      this.hours = "" + hour;

      //Update end value of Interval entity every 10 sec
      if (sec % 10 == 0 || sec == 0) {
        var url = this.baseUrl + "api/interval/end/element/" + this.elementId;
        this.http.
          post(url, null).subscribe(result => {
            console.log("End timing element" + this.elementId + " has been set.");
          }, error => console.error(error));
      }
    });
    console.log("timerSubscription for NodeElement " + this.elementId + " has been set");
  }

  StopTimer() {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
      console.log("Unsubscribe from timing with element " + this.elementId + " has been done.");
    }
  }

}
