import { Component, Input, Inject } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../service/auth.service';


@Component({
  selector: 'project-form',
  templateUrl: './project-form.component.html',
  styleUrls: ['./project-form.component.less']
})

export class ProjectFormComponent {
  nodeElement: NodeElement;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string) {

    // create an empty object from the NodeElement interface
    this.nodeElement = <NodeElement>{};
    var id = +this.activatedRoute.snapshot.params["id"];
    console.log(id);

    if (id) {
      var url = this.baseUrl + "api/project/" + id;
      this.http.get<NodeElement>(url).subscribe(result => {
        this.nodeElement = result;
      }, error => console.error(error));
    }
    else {
      console.log("Invalid id: routing back to home...");
      this.router.navigate(["home"]);
    }
  }

  onEdit() {
    this.router.navigate(["project/edit", this.nodeElement.Id]);
  }

  onDelete() {
    if (confirm("Do you really want to delete this element?")) {
      var url = this.baseUrl + "api/project/" + this.nodeElement.Id;
      this.http
        .delete(url)
        .subscribe(res => {
          console.log("Element " + this.nodeElement.Id + " has been deleted.");
          this.router.navigate(["projects"]);
        }, error => console.log(error));
    }
  }
}
