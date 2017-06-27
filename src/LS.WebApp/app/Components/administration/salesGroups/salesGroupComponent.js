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
var router_1 = require("@angular/router");
var Services_1 = require("../../../Service/Services");
require("rxjs/add/operator/mergeMap");
var global_1 = require("../../../Shared/global");
var angular2_toaster_1 = require("angular2-toaster");
//https://www.npmjs.com/package/ng2-accordion
var SalesGroupComponent = (function () {
    function SalesGroupComponent(router, _global, _salesGroupDataService, toasterService, _constants) {
        this.router = router;
        this._global = _global;
        this._salesGroupDataService = _salesGroupDataService;
        this.toasterService = toasterService;
        this._constants = _constants;
        this.hasLoaded = false;
        this.unassingedManager = false;
    }
    SalesGroupComponent.prototype.ngOnInit = function () {
        this.getAndFormatSalesGroups();
    };
    SalesGroupComponent.prototype.getAndFormatSalesGroups = function () {
        var _this = this;
        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error retrieving groups.', response.error.userHelp);
                _this.hasLoaded = true;
            }
            _this.salesGroups = response.data;
            if (_this.salesGroups.length < 1) {
                _this.unassingedManager = true;
            }
            _this.hasLoaded = true;
        }, function (error) { return _this.msg = error; });
    };
    return SalesGroupComponent;
}());
SalesGroupComponent = __decorate([
    core_1.Component({
        selector: 'sales-groups',
        templateUrl: 'app/Components/administration/salesGroups/salesGroups.html',
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [router_1.Router, global_1.Global, Services_1.SalesGroupDataService, angular2_toaster_1.ToasterService, global_1.Constants])
], SalesGroupComponent);
exports.SalesGroupComponent = SalesGroupComponent;
//# sourceMappingURL=salesGroupComponent.js.map