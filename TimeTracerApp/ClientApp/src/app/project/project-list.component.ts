import { Component, Inject, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Router } from "@angular/router";

@Component({
  selector: "project-list",
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.less']
})


export class ProjectListComponent implements OnInit {
  @Input() class: string;
  title: string;
  selectedNodeElement: NodeElement;
  nodeElements: NodeElement[];

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string, private router: Router) {
  }

  ngOnInit() {
    var url = this.baseUrl + "api/project/root/";

    this.title = "Project list";
    this.http.get<NodeElement[]>(url).subscribe(result => {
      this.nodeElements = result;
      
    }), error => console.error(error);
  }

  onSelect(nodeElement: NodeElement) {
    this.selectedNodeElement = nodeElement;
    console.log("Element with ID" + this.selectedNodeElement.Id + " has been selected");
    this.router.navigate(["project/form", this.selectedNodeElement.Id]);
  }
}
