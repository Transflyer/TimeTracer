import { Component, Input, Inject, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../service/auth.service';
import { StopwatchComponent } from '../stopwatch/stopwatch.component';
import { ElementService } from '../service/element.service';

@Component({
  selector: 'element-active',
  templateUrl: './element-active.component.html',
  styleUrls: ['./element-active.component.less']
})

export class ElementActiveComponent implements OnInit {
  activeElement: NodeElement;
  @ViewChild(StopwatchComponent) stopWatchChild: StopwatchComponent;

  constructor(private http: HttpClient,
    private router: Router,
    private elementChanged: ElementService,
    @Inject('BASE_URL') private baseUrl: string) { }

  ngOnInit() {

    this.elementChanged.change.subscribe(IsChanged => {
      this.getActiveElement();
    });

    this.getActiveElement();
  }

  getActiveElement() {
    var url = this.baseUrl + "api/elements/active";
    this.activeElement = <NodeElement>{};
    this.activeElement.Title = "";

    this.http.get<NodeElement>(url).subscribe(result => {
      this.activeElement = result;

    }, error => {
      if (error.status == 404) this.activeElement.Title = 'No active element';
      console.error(error);
    });
  }

  moveToActive(id: number) {
    this.router.navigate(["element", id]);
  }
}
