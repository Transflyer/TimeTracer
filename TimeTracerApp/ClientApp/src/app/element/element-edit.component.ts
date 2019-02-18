import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { error } from 'util';

@Component({
  selector: 'element-edit',
  templateUrl: './element-edit.component.html',
  styleUrls: ['./element-edit.component.less']
})
export class ElementEditComponent {

  title: string;
  nodeElement: NodeElement;
  form: FormGroup;

  //if edit existing Node
  editMode: boolean;

  constructor(
    private activeRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {

    //create empty NodeElement
    this.nodeElement = <NodeElement>{};

    //initialize the form
    this.createForm();

    var id = +this.activeRoute.snapshot.params["id"];
    
    if (id) {

      this.editMode = true;

      //fetch the NodeElement from the server
      var url = this.baseUrl + "api/elements/" + id;
      this.http.get<NodeElement>(url).subscribe(result => {
        this.nodeElement = result;
        this.title = "Edit - " + this.nodeElement.Title;

        this.updateForm();
      }), error => console.error(error);
    }
    else {
      this.editMode = false;
      this.title = "Create a new element";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Title: ['', Validators.required],
      Description: ''
    });
  }

  updateForm() {
    this.form.setValue({
      Title: this.nodeElement.Title,
      Description: this.nodeElement.Description || ''
    });
  }

  onSubmit() {

    var tempNodeElement = <NodeElement>{};
    tempNodeElement.Title = this.form.value.Title;
    tempNodeElement.Description = this.form.value.Description;
    tempNodeElement.ParentId = +this.activeRoute.snapshot.params["parentid"];

    var url = this.baseUrl + "api/elements";

    if (this.editMode) {
      tempNodeElement.Id = this.nodeElement.Id;

      this.http
        .post<NodeElement>(url, tempNodeElement)
        .subscribe(result => {
          this.nodeElement = result;
          console.log("Node element" + this.nodeElement.Id + "has been updated.");
          this.router.navigate(["projects"]);
        }, error => console.error(error));
    }
    else {
      this.http
        .put<NodeElement>(url, tempNodeElement)
        .subscribe(result => {
          var v = result;
          console.log("Node element" + v.Id + "has been created");
          this.router.navigate(["projects"]);
        }), error => console.error(error);
    }
  }

  onBack() {
    this.router.navigate(["projects"]);
  }

  // retrieve a FormControl
  getFormControl(name: string) {
    return this.form.get(name);
  }

  // returns TRUE if the FormControl is valid
  isValid(name: string) {
    var e = this.getFormControl(name);
    return e && e.valid;
  }

  // returns TRUE if the FormControl has been changed
  isChanged(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched);
  }

  // returns TRUE if the FormControl is invalid after user changes
  hasError(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched) && !e.valid;
  }

}

