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


export class ProjectComponent {
  title: string;
  selectedNodeElement: nodeElement;
  nodeElements: nodeElement[];

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {
    this.nodeElements = <nodeElement[]>{};
    var url = this.baseUrl + "api/project/ByTitle/";
    console.log(url);
    this.title = "My project";
    this.http.get<nodeElement[]>(url).subscribe(result => {
      this.nodeElements = result;
    }), error => console.error(error);
  }
}
