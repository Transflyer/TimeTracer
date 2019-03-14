import { Component, OnInit, Input, Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Console } from '@angular/core/src/console';
import { Router } from "@angular/router";

@Component({
  selector: 'interval-list',
  templateUrl: './interval-list.component.html',
  styleUrls: ['./interval-list.component.less']
})
export class IntervalListComponent implements OnInit {
  @Input() nodeElementId: number;
  elementIntervals: IntervalElement[];
  editInterval: boolean;
  editIntervalObject: IntervalElement;

  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit() {
    this.editInterval = false;
    this.updateIntervalList(this.nodeElementId);
  }

  updateIntervalList(id: number) {
    var url = this.baseUrl + "api/timespent/" + this.nodeElementId;
    this.http.get<IntervalElement[]>(url).subscribe(result => {
      this.elementIntervals = result;
    }), error => console.error(error);
  }

  onEdit(model: IntervalElement) {
    if (model) {
      this.editIntervalObject = model;
      this.editInterval = true;
    }
  }
}

