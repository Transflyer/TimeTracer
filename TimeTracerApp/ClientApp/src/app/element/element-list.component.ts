import { Component, Inject, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
  selector: "element-list",
  templateUrl: './element-list.component.html',
  styleUrls: ['./element-list.component.less']
})


export class ElementListComponent implements OnInit {
  @Input() parentId: number;
  @Input() class: string;
  title: string;
  
  selectedNodeElement: NodeElement;
  nodeElements: NodeElement[];
  elementsCount: number;

  constructor(private activetedRoute: ActivatedRoute,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string, private router: Router) {
  }

  ngOnInit() {
    this.elementsCount = 0;
    this.updateList(this.parentId);
    this.activetedRoute.params.subscribe(params => {
      var id = params["id"];
      if (id) {
        this.updateList(id);
      }
    })
  }

  updateList(id: number) {
    var url = this.baseUrl + "api/elements/root/" + id;
    this.title = "Elements list";
    this.http.get<NodeElement[]>(url).subscribe(result => {
      this.nodeElements = result;
      this.elementsCount = result.length;
    }), error => console.error(error);
  }

  onSelect(nodeElement: NodeElement) {
    this.selectedNodeElement = nodeElement;
    console.log("Element with ID" + this.selectedNodeElement.Id + " has been selected");
    this.updateList(this.selectedNodeElement.Id);
    this.router.navigate(["element", this.selectedNodeElement.Id]);
  }
}
