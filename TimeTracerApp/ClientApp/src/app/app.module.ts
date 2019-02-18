import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AuthService } from '../app/service/auth.service';
import { AuthInterceptor } from './service/auth.interceptor';
import { AuthResponseInterceptor } from './service/auth.response.interceptor';
import { AfterIfDirective } from './service/after-if.directive';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

import { HomeComponent } from './home/home.component';
import { ProjectListComponent } from './project/project-list.component';
import { ProjectEditComponent } from './project/project-edit.component';
import { ProjectsComponent } from './project/projects.component';
import { ElementComponent } from './element/element.component';
import { ElementEditComponent } from './element/element-edit.component';
import { ElementListComponent } from './element/element-list.component';

import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './user/register.component';
import { AboutComponent } from './about/about.component';

import { PageNotFoundComponent } from './pagenotfound/pagenotfound.component';




@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AboutComponent,
    ProjectsComponent,
    ProjectListComponent,
    LoginComponent,
    RegisterComponent,
    PageNotFoundComponent,
    AfterIfDirective,
    ProjectEditComponent,
    ElementComponent,
    ElementEditComponent,
    ElementListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      { path: 'about', component: AboutComponent },
      { path: 'projects', component: ProjectsComponent },
      { path: 'element/:id', component: ElementComponent },
      { path: 'element/edit/:id/:parentid', component: ElementEditComponent },
      { path: 'project/create', component: ProjectEditComponent },
      { path: 'project/create/:id', component: ProjectEditComponent },
      { path: 'project/edit/:id', component: ProjectEditComponent },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: '**', component: PageNotFoundComponent }
    ])
  ],
  providers: [
    AuthService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthResponseInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
