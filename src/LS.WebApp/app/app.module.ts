//
import { NgModule } from '@angular/core';
import { APP_BASE_HREF } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
//import { Ng2Bs3ModalModule } from 'ng2-bs3-modal/ng2-bs3-modal';
import { HttpModule } from '@angular/http';
import { MyDatePickerModule } from 'mydatepicker';
import { routing } from './app.routing';
//System
import { ToasterModule, ToasterService,ToasterConfig } from 'angular2-toaster';
import { Global } from './Shared/global';
import { Constants } from './Shared/global';
import { Accordion, AccordionGroup, AccordionHead } from './Shared/accordion';
import { YesNo } from './Shared/filters';
import { AgGridModule } from 'ag-grid-angular/main';
import { DatePipe } from '@angular/common';
//Services Imports
import { LoggedInUserResolve} from './Service/resolve.service'
import { CompanyDataService } from './Service/Services';
import { MessageDataService } from './Service/Services';
import { AppUserDataService } from './Service/Services';
import { SalesGroupDataService } from './Service/Services';
import { SalesTeamDataService } from './Service/Services';
import { EmailValidator } from './Shared/directives';

//Component Imports
import { HomeComponent } from './components/home/home.component';
import { HeaderComponent } from "./common/header"
import { BrandingComponent } from './common/branding';
import { UserComponent } from './components/administration/users/users.component';
import { modifyUsersComponent } from './components/administration/users/modifyUsers.component';
import { SalesGroupComponent } from './components/administration/salesGroups/salesGroupComponent';
import { UsersdetailComponent } from './components/administration/users/users-detail.component';
import { LoginMsgComponent } from './components/administration/loginMessages/loginMsg.component';
import { ModifyLoginMsgComponent } from './components/administration/loginMessages/modifyLoginMsg.component';
import { ModalModule, DatepickerModule } from 'ngx-bootstrap';
// Ng2Bs3ModalModule,
@NgModule({
    imports: [BrowserModule, ModalModule.forRoot(), DatepickerModule.forRoot(), MyDatePickerModule, ReactiveFormsModule,  FormsModule, HttpModule, routing, ToasterModule, AgGridModule.withComponents([]),],

    declarations: [AppComponent, SalesGroupComponent, EmailValidator, modifyUsersComponent, LoginMsgComponent, YesNo, Accordion, AccordionGroup, AccordionHead, UsersdetailComponent, HomeComponent, HeaderComponent, BrandingComponent, UserComponent,ModifyLoginMsgComponent],

    providers: [{ provide: APP_BASE_HREF, useValue: '/' }, DatePipe, CompanyDataService, SalesTeamDataService, SalesGroupDataService, AppUserDataService,LoggedInUserResolve, Global, Constants, MessageDataService,],

	bootstrap: [AppComponent]

})
export class AppModule { }