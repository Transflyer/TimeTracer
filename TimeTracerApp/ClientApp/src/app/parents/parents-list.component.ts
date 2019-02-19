import { Component, Inject, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ActivatedRoute, Router } from "@angular/router";


@Component({
  selector: 'parents-list',
  templateUrl: './parents-list.component.html',
  styleUrls: ['./parents-list.component.less']
})


export class ParentsListComponent implements OnInit {
  @Input() childId: number;
  @Input() class: string;
  title: string;

  selectedNodeElement: NodeElement;
  nodeElements: NodeElement[];
  
  constructor(private activetedRoute: ActivatedRoute,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string, private router: Router) { }

  ngOnInit() {
    this.updateList(this.childId);
    this.activetedRoute.params.subscribe(params => {
      var id = params["id"];
      if (id) {
        this.updateList(id);
      }
    })
  }

  updateList(id: number) {
    var url = this.baseUrl + "api/elements/parents/" + id;
    this.title = "Parents list chain";
    this.http.get<NodeElement[]>(url).subscribe(result => {
      this.nodeElements = result;
    }), error => console.error(error);
  }

  onSelect(nodeElement: NodeElement) {
    this.selectedNodeElement = nodeElement;
    console.log("Element with ID" + this.selectedNodeElement.Id + " has been selected");
    this.updateList(this.selectedNodeElement.Id);
    this.router.navigate(["element", this.selectedNodeElement.Id]);
  }

}


