import { Component, OnInit, Inject, Input } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Route } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { error } from 'util';
import { validateConfig } from '@angular/router/src/config';

@Component({
  selector: 'interval-edit',
  templateUrl: './interval-edit.component.html',
  styleUrls: ['./interval-edit.component.less']
})

export class IntervalEditComponent implements OnInit {
  title: string;
  @Input() interval: IntervalElement;
  updatedInterval: IntervalElement;
  form: FormGroup;

  constructor(
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit() {
    this.title = "Edit interval";

    //Copy to object to update
    this.updatedInterval = <IntervalElement>JSON.parse(JSON.stringify(this.interval));
    this.createForm();
  }

  createForm() {
    this.form = this.fb.group({
      Hours: ['', Validators.required],
      Minutes: [''],
      Seconds: ['']
    });
    this.form.setValue({
      Hours: this.updatedInterval.Hours,
      Minutes: this.updatedInterval.Minutes,
      Seconds: this.updatedInterval.Seconds
    })
  }

  onSubmit() {
    this.updatedInterval.Hours = this.form.value.Hours;
    this.updatedInterval.Minutes = this.form.value.Minutes;
    this.updatedInterval.Seconds = this.form.value.Seconds;

    var url = this.baseUrl + "api/interval/updateend/element/";

    this.http.post<IntervalElement>(url, this.updatedInterval).subscribe(result => {
      this.interval = result;
      if (this.interval.TotalSecond = this.updatedInterval.TotalSecond) {
        console.log("Interval " + this.interval.Id + " has been updated!")
      }
    }, error => console.error(error));

    this.router.navigate(["element", this.interval.ElementId]);
    
  }
}
