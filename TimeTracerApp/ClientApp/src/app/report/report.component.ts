import { Component, OnInit, Inject, Input } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../service/auth.service';
import { error } from 'protractor';

@Component({
  selector: 'report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.less']
})
export class ReportComponent implements OnInit {
  @Input() ReportModel: Report[];
  @Input() ShowTitle: boolean;
  @Input() Shift: number;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit() {
    this.Shift += 1;
  }

}
