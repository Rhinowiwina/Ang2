"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
//
var core_1 = require("@angular/core");
var common_1 = require("@angular/common");
var platform_browser_1 = require("@angular/platform-browser");
var forms_1 = require("@angular/forms");
var forms_2 = require("@angular/forms");
var app_component_1 = require("./app.component");
//import { Ng2Bs3ModalModule } from 'ng2-bs3-modal/ng2-bs3-modal';
var http_1 = require("@angular/http");
var mydatepicker_1 = require("mydatepicker");
var app_routing_1 = require("./app.routing");
//System
var angular2_toaster_1 = require("angular2-toaster");
var global_1 = require("./Shared/global");
var global_2 = require("./Shared/global");
var accordion_1 = require("./Shared/accordion");
var filters_1 = require("./Shared/filters");
var main_1 = require("ag-grid-angular/main");
var common_2 = require("@angular/common");
//Services Imports
var resolve_service_1 = require("./Service/resolve.service");
var Services_1 = require("./Service/Services");
var Services_2 = require("./Service/Services");
var Services_3 = require("./Service/Services");
//Component Imports
var home_component_1 = require("./components/home/home.component");
var header_1 = require("./common/header");
var branding_1 = require("./common/branding");
var users_component_1 = require("./components/administration/users/users.component");
var users_detail_component_1 = require("./components/administration/users/users-detail.component");
var loginMsg_component_1 = require("./components/administration/loginMessages/loginMsg.component");
var modifyLoginMsg_component_1 = require("./components/administration/loginMessages/modifyLoginMsg.component");
var ngx_bootstrap_1 = require("ngx-bootstrap");
// Ng2Bs3ModalModule,
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    core_1.NgModule({
        imports: [platform_browser_1.BrowserModule, ngx_bootstrap_1.ModalModule.forRoot(), ngx_bootstrap_1.DatepickerModule.forRoot(), mydatepicker_1.MyDatePickerModule, forms_2.ReactiveFormsModule, forms_1.FormsModule, http_1.HttpModule, app_routing_1.routing, angular2_toaster_1.ToasterModule, main_1.AgGridModule.withComponents([]),],
        declarations: [app_component_1.AppComponent, loginMsg_component_1.LoginMsgComponent, filters_1.YesNo, accordion_1.Accordion, accordion_1.AccordionGroup, accordion_1.AccordionHead, users_detail_component_1.UsersdetailComponent, home_component_1.HomeComponent, header_1.HeaderComponent, branding_1.BrandingComponent, users_component_1.UserComponent, modifyLoginMsg_component_1.ModifyLoginMsgComponent],
        providers: [{ provide: common_1.APP_BASE_HREF, useValue: '/' }, common_2.DatePipe, Services_1.CompanyDataService, Services_3.AppUserDataService, resolve_service_1.LoggedInUserResolve, global_1.Global, global_2.Constants, Services_2.MessageDataService,],
        bootstrap: [app_component_1.AppComponent]
    })
], AppModule);
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map