import { NgModule } from '@angular/core';
import { APP_BASE_HREF } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { Ng2Bs3ModalModule } from 'ng2-bs3-modal/ng2-bs3-modal';
import { HttpModule } from '@angular/http';

import { routing } from './app.routing';
//System
import { ToasterModule, ToasterService,ToasterConfig } from 'angular2-toaster';
import { Global } from './Shared/global';
import { Constants } from './Shared/global';


//Services Imports
import { CompanyDataService } from './Service/Services';
import { MessageDataService } from './Service/Services';
import { AppUserDataService } from './Service/Services';
//Component Imports
import { HomeComponent } from './components/home/home.component';
import { HeaderComponent } from "./common/header"
import { BrandingComponent } from './common/branding';
import { UserComponent } from './components/administration/users/users.component';
import { UsersdetailComponent } from './components/administration/users/users-detail.component';
import {LoggedInUserResolve } from './Service/resolve.service';
@NgModule({
	imports: [BrowserModule, ReactiveFormsModule, HttpModule, routing, Ng2Bs3ModalModule, ToasterModule,],

	declarations: [AppComponent, UsersdetailComponent,HomeComponent, HeaderComponent, BrandingComponent, UserComponent],

	providers: [{ provide: APP_BASE_HREF, useValue: '/' }, CompanyDataService, AppUserDataService,LoggedInUserResolve, Global, Constants, MessageDataService,],

	bootstrap: [AppComponent]

})
export class AppModule { }