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
// branding.resolve.service.ts
//http://shermandigital.com/blog/wait-for-data-before-rendering-views-in-angular-2/
//https://blog.thoughtram.io/angular/2016/10/10/resolving-route-data-in-angular-2.html
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var Services_1 = require("../Service/Services");
var BrandingResolve = (function () {
    function BrandingResolve(_brandingService, router) {
        this._brandingService = _brandingService;
        this.router = router;
    }
    BrandingResolve.prototype.resolve = function (route) {
        var id = +route.params['id'];
        return this._brandingService.get("api/company/getCompany?companyId=65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c");
    };
    return BrandingResolve;
}());
BrandingResolve = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [Services_1.CompanyDataService, router_1.Router])
], BrandingResolve);
exports.BrandingResolve = BrandingResolve;
//# sourceMappingURL=branding.resolve.service.js.map