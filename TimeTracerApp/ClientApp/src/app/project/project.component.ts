import { Component, Inject, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { forEach } from "@angular/router/src/utils/collection";
import { request } from "https";
import { Router } from "@angular/router";
import { error } from "protractor";

@Component({
  selector: "project",
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.css']
})

export class ProjectComponent implements OnInit {
  @Input() class: string;
  title: string;
  selectedNodeElement: NodeElement;
  NodeElements: NodeElement[];

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {  }

  ngOnInit() {
    console.log("ProjectComponent " +
      " instantiated with the following class: "
      + this.class);
    var url = this.baseUrl + "api/project/";
    this.title = "My project";
    this.http.get<NodeElement[]>(url).subscribe(result => {
      this.NodeElements = result;
    }), error => console.error(error);
  }
}
