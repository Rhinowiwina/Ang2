"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
//Toast Selector is put here and accessible by all components. Default config is set in toastr-config.js
//https://github.com/Stabzs/Angular2-Toaster/blob/master/README.md
//error: 'icon-error',
//info: 'icon-info',
//wait: 'icon-wait',
//success: 'icon-success',
//warning: 'icon-warning'
var core_1 = require("@angular/core");
var Services_1 = require("./Service/Services");
var angular2_toaster_1 = require("angular2-toaster");
var Services_2 = require("./Service/Services");
require("rxjs/add/operator/mergeMap");
var global_1 = require("./Shared/global");
var AppComponent = (function () {
    function AppComponent(toasterService, _companyDataService, _appUserDataService, _global) {
        this._companyDataService = _companyDataService;
        this._appUserDataService = _appUserDataService;
        this._global = _global;
        this.toasterconfig = new angular2_toaster_1.ToasterConfig({
            limit: 4,
            positionClass: 'toast-bottom-right',
            showCloseButton: true,
            tapToDismiss: false,
            bodyOutputType: angular2_toaster_1.BodyOutputType.TrustedHtml,
        });
        this.toasterService = toasterService;
    }
    AppComponent.prototype.ngOnInit = function () {
        this._global.criticalMsgRead = true;
        this.GetBranding();
    };
    //popToast() {
    //	var toast = {
    //		type: 'error',
    //		title: 'Here is a Toast Title',
    //		body: '<h1>Here is a Toast Body</h1>',
    //		showCloseButton: true,
    //		//positionClass: 'toast-top-left',
    //	};
    //	this.toasterService.pop(toast);
    //	//this.toasterService.pop('error', 'Error', 'Big ol');
    //}
    AppComponent.prototype.GetBranding = function () {
        var _this = this;
        this._companyDataService.getCompany("65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c").subscribe(function (response) {
            var response = response;
            //console.log(response)
            //we use alert here because toastr container is not available until html has rendered.
            //everywhere else toastr will be available.
            if (!response.isSuccessful) {
                alert(response.error.userHelp);
                return;
            }
            _this.branding = response.data;
            _this._appUserDataService.getLoggedInUser().subscribe(function (response) {
                var response = response;
                _this.loggedInUser = response.data;
                _this._global.loggedInUser = _this.loggedInUser;
                if (!response.isSuccessful) {
                    alert(response.error.userHelp);
                    return;
                }
            }, function (error) { return _this.msg = error; });
        }, function (error) { return _this.msg = error; });
        if (this.msg != null) {
            alert(this.msg);
        }
    };
    return AppComponent;
}());
AppComponent = __decorate([
    core_1.Component({
        selector: "my-app",
        template: "\t <div *ngIf=\"branding && loggedInUser\" >\n\t\t      <app-header [brandingmodel]=\"branding\" [loggedInUser]=\"loggedInUser\"> \n              </app-header></div>\n     <toaster-container [toasterconfig]=\"toasterconfig\"></toaster-container>\n \n            <div class='container container-content panel panel-default'>\n                <router-outlet></router-outlet>\n            </div>\n\t\t\t",
        styleUrls: ['app/app.component.css'],
        encapsulation: core_1.ViewEncapsulation.None,
    }),
    __metadata("design:paramtypes", [angular2_toaster_1.ToasterService, Services_1.CompanyDataService, Services_2.AppUserDataService, global_1.Global])
], AppComponent);
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map