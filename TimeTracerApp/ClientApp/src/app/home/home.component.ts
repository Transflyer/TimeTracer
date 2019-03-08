import { Component, Input, Inject, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../service/auth.service';
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {
  ReportModel: Report[];

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string) {

  }
  ngOnInit() {
    var url = this.baseUrl + "api/report/user";
    this.http.get<Report[]>(url).subscribe(result => {
      this.ReportModel = result;
      console.log("We get report" + this.ReportModel[0].NodeElementTitle);
    }, error => console.log(error));
  }
}
