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
var Services_1 = require("..//Service/Services");
var BrandingComponent = (function () {
    function BrandingComponent(_brandingService) {
        this._brandingService = _brandingService;
    }
    BrandingComponent.prototype.ngOnInit = function () {
        this.GetBranding();
    };
    BrandingComponent.prototype.GetBranding = function () {
        //this._brandingService.get("api/company/getCompany?companyId=65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c")
        //	.subscribe(branding => {
        //		this.branding = branding.Data;
        //            console.log(this.branding);
        //	}, error => this.msg = <any>error);
    };
    return BrandingComponent;
}());
BrandingComponent = __decorate([
    core_1.Component({
        selector: 'app-branding',
        templateUrl: 'app/common/branding.html',
        providers: [Services_1.CompanyDataService]
    }),
    __metadata("design:paramtypes", [Services_1.CompanyDataService])
], BrandingComponent);
exports.BrandingComponent = BrandingComponent;
//# sourceMappingURL=branding.js.map