import { Component, Input, Inject, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../service/auth.service';
import { error } from "protractor";
import { StopwatchComponent } from '../stopwatch/stopwatch.component';

@Component({
  selector: 'element',
  templateUrl: './element.component.html',
  styleUrls: ['./element.component.less']
})

export class ElementComponent implements OnInit {
  nodeElement: NodeElement;
  parentId: number;
  currentId: number;
  @ViewChild(StopwatchComponent) stopWatchChild: StopwatchComponent;

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
        console.log("Element with id" + this.currentId + " has been updated");
      }
    })

 }

  updateElement(id: number) {
    this.parentId = id;
    var url = this.baseUrl + "api/elements/" + id;
    this.http.get<NodeElement>(url).subscribe(result => {
      this.nodeElement = result;
    }, error => console.error(error));
   
    //url = this.baseUrl + "api/timespent/element/" + id;
    //this.http.get<TimeSpanElement>(url).subscribe(result => {
    //  this.elementTimeSpan = result;
    //}, error => console.error(error));
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

  onStart() {
    var url = this.baseUrl + "api/timespent/" + this.nodeElement.Id;
    this.http.
      put(url, null).subscribe(result => {
        console.log("TimeSpents for " + this.nodeElement.Id + " has been created.");
        this.stopWatchChild.StartTimer();
      }, error => console.error(error));
  }

  onStop()
  {
    var url = this.baseUrl + "api/timespent/end/element/" + this.nodeElement.Id;
    this.http.
      post(url, null).subscribe(result => {
        console.log("End timing for NodeElement " + this.nodeElement.Id + " has been set.");
        this.stopWatchChild.StopTimer();
      }, error => {
        console.error(error);
        this.stopWatchChild.StopTimer();
      });
    
  }
}
