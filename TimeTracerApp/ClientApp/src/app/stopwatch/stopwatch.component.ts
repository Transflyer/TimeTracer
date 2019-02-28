import { Component, Input, Inject, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../service/auth.service';
import { Observable } from "rxjs";
import { HOST_ATTR } from "@angular/platform-browser/src/dom/dom_renderer";


@Component({
  selector: 'stopwatch',
  templateUrl: './stopwatch.component.html',
  styleUrls: ['./stopwatch.component.less']
})
export class StopwatchComponent implements OnInit {
  elementTimeSpan: TimeSpanElement;
  elementId: number;
  IsOpen: boolean;
  days: string;
  hours: string;
  minutes: string;
  seconds: string;


  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string
  ) { }

  ngOnInit() {

    var id = +this.activatedRoute.snapshot.params["id"];
    this.activatedRoute.params.subscribe(params => {
      var id = params["id"];
      if (id) {
        this.elementId = id;
        this.getElementTimeSpan();
      }
    })

    if (id) {
      this.elementId = id;
      this.getElementTimeSpan();
    }

    else {
      console.log("Cant get Element Time Span");
    }
  }

  getElementTimeSpan() {
    var url = this.baseUrl + "api/timespent/element/" + this.elementId;
    this.http.get<TimeSpanElement>(url).subscribe(result => {
      this.elementTimeSpan = result;
      if (this.elementTimeSpan.IsOpenTimeSpentId != 0) this.StartTimer();
      else {
        this.days = "" + this.elementTimeSpan.Days;
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
    let timer = Observable.timer(2000, 1000);
    timer.subscribe(() => {

      //Update timer every second
      var sec = this.elementTimeSpan.Seconds;
      var min = this.elementTimeSpan.Minutes;
      var hour = this.elementTimeSpan.Hours;
      var days = this.elementTimeSpan.Days;

      sec++;
      if (sec == 60) {
        min++;
        if (min == 60) {
          hour++;
          if (hour == 24) {
            days++;
            hour = 0;
            this.elementTimeSpan.Days = days;
          }
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
      this.days = "" + days;


      //Update end value of TimeSpent entity every 10 sec
      if (sec % 10 == 0 || sec == 0) {
        var url = this.baseUrl + "api/timespent/updateend/element/" + this.elementId;
        this.http.
          post(url, null).subscribe(result => {
            console.log("End timing element" + this.elementId + " has been set.");
          }, error => console.error(error));
      }
    });
  }

}
