import { Component, Inject, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Router } from "@angular/router";

@Component({
  selector: "element-list",
  templateUrl: './element-list.component.html',
  styleUrls: ['./element-list.component.less']
})


export class ElementListComponent implements OnInit {
  @Input() parentId: string;
  @Input() class: string;
  title: string;
  
  selectedNodeElement: NodeElement;
  nodeElements: NodeElement[];

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string, private router: Router) {
  }

  ngOnInit() {
    var url = this.baseUrl + "api/elements/root/"+this.parentId;
   
    this.title = "Element list";
    this.http.get<NodeElement[]>(url).subscribe(result => {
      this.nodeElements = result;

    }), error => console.error(error);
  }

  onSelect(nodeElement: NodeElement) {
    this.selectedNodeElement = nodeElement;
    console.log("Element with ID" + this.selectedNodeElement.Id + " has been selected");
    this.router.navigate(["element", this.selectedNodeElement.Id]);
  }
}
