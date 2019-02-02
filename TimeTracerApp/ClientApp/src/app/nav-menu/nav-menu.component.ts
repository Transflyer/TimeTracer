import { Component, OnChanges, SimpleChanges} from '@angular/core';
import { AuthService } from '../service/auth.service';
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})


export class NavMenuComponent implements OnChanges{
  user: User;
  email: String
  constructor(
    public auth: AuthService,
    private router: Router,
    private http: HttpClient) {
    this.email = "";
    this.getUser();
  }

  getUser() {
    //Get User data
    this.http.get<User>('api/user')
      .subscribe(result => {
        this.user = result;
        this.email = this.user.Email;
        console.log("We get user!")
      }, error => console.log(error));
  }

  ngOnChanges(changes: SimpleChanges) {
    this.getUser();
  }

logout(): boolean {
    // logs out the user, then redirects him to Home View.
    if (this.auth.logout()) {
      this.router.navigate([""]);
      this.email = "";
    }
    return false;
  }

  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}

