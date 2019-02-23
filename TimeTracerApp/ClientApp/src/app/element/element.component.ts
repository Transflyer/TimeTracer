import { Component, Input, Inject, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'element',
  templateUrl: './element.component.html',
  styleUrls: ['./element.component.less']
})

export class ElementComponent implements OnInit {
  nodeElement: NodeElement;
  parentId: number;
  currentId: number;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit() {
    // create an empty object from the NodeElement interface
    this.nodeElement = <NodeElement>{};
    var id = +this.activatedRoute.snapshot.params["id"];
    this.activatedRoute.params.subscribe(params => {
      var id = params["id"];
      if (id) {
        this.currentId = id;
        this.updateElement(id);
      }
    })

    if (id) {
      this.currentId = id;
      this.updateElement(id);
    }
    else {
      console.log("Invalid id: routing back to home...");
      this.router.navigate(["home"]);
    }
  }

  updateElement(id: number) {
    this.parentId = id;
    var url = this.baseUrl + "api/elements/" + id;
    this.http.get<NodeElement>(url).subscribe(result => {
      this.nodeElement = result;
    }, error => console.error(error));
  }

  onEdit() {
    this.router.navigate(["element/edit", this.nodeElement.Id]);
  }

  onAdd() {
    this.router.navigate(["element/edit", 0, this.nodeElement.Id]);
  }

  onDelete() {
    if (confirm("Do you really want to delete this element?")) {
      var url = this.baseUrl + "api/elements/" + this.nodeElement.Id;
      this.http
        .delete(url)
        .subscribe(res => {
          console.log("Element " + this.nodeElement.Id + " has been deleted.");
          this.router.navigate(["projects"]);
        }, error => console.log(error));
    }
  }
}
