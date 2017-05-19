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
var core_1 = require("@angular/core");
var branding_service_1 = require("./Service/branding.service");
var AppComponent = (function () {
    function AppComponent(_brandingService) {
        this._brandingService = _brandingService;
    }
    AppComponent.prototype.ngOnInit = function () {
        this.GetBranding();
        this.test = "atest";
    };
    AppComponent.prototype.GetBranding = function () {
        var _this = this;
        this._brandingService.get("api/company/getCompany?companyId=65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c")
            .subscribe(function (branding) {
            _this.branding = branding.Data;
            return _this.branding;
        }, function (error) { return _this.msg = error; });
    };
    return AppComponent;
}());
AppComponent = __decorate([
    core_1.Component({
        selector: "my-app",
        template: "\t\n\t\t\t<div *ngIf=\"this.branding\">\n\t\t      <app-header [brandingmodel]=\"branding\"> \n          \n            \n        </app-header></div>\n            <div class='container'>\n                <router-outlet></router-outlet>\n            </div>\n\t\t\t",
        providers: [branding_service_1.BrandingService],
    }),
    __metadata("design:paramtypes", [branding_service_1.BrandingService])
], AppComponent);
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map