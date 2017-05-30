﻿import { NgModule } from '@angular/core';
import { APP_BASE_HREF } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { Ng2Bs3ModalModule } from 'ng2-bs3-modal/ng2-bs3-modal';
import { HttpModule } from '@angular/http';

import { routing } from './app.routing';
//import { UserComponent } from './components/user.component';
import { HomeComponent } from './components/home/home.component';
import { HeaderComponent } from "./common/header"
import { BrandingComponent } from './common/branding';

import { CompanyDataService } from './Service/Services';
import { MessageDataService } from './Service/Services';

import { ToasterModule, ToasterService,ToasterConfig } from 'angular2-toaster';
import { Global} from './Shared/global';
import { AppUserDataService } from './Service/Services';
import { BrandingResolve } from './Service/branding.resolve.service';
@NgModule({
	imports: [BrowserModule, ReactiveFormsModule, HttpModule, routing, Ng2Bs3ModalModule,ToasterModule,],
	declarations: [AppComponent,  HomeComponent, HeaderComponent, BrandingComponent],
	providers: [{ provide: APP_BASE_HREF, useValue: '/' },CompanyDataService, AppUserDataService, BrandingResolve, Global, MessageDataService,],
	bootstrap: [AppComponent]

})
export class AppModule { }