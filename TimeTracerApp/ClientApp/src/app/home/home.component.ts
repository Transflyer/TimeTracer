import { Component, Input, Inject, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {

  constructor(public auth: AuthService,
    ) {

  }
  ngOnInit() {

  }
}
