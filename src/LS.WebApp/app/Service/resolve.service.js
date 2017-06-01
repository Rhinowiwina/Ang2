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
var Observable_1 = require("rxjs/Observable");
var router_1 = require("@angular/router");
var Services_1 = require("../Service/Services");
var Global_1 = require("../Shared/Global");
var LoggedInUserResolve = (function () {
    function LoggedInUserResolve(_global, router) {
        this._global = _global;
        this.router = router;
    }
    LoggedInUserResolve.prototype.resolve = function (route) {
        var id = +route.params['id'];
        return this._global.loggedInUser;
    };
    return LoggedInUserResolve;
}());
LoggedInUserResolve = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [Global_1.Global, router_1.Router])
], LoggedInUserResolve);
exports.LoggedInUserResolve = LoggedInUserResolve;
var GlobalVariableResolve = (function () {
    function GlobalVariableResolve(_appUserDataService, router) {
        this._appUserDataService = _appUserDataService;
        this.router = router;
        alert('tere39');
    }
    GlobalVariableResolve.prototype.resolve = function (route) {
        var id = +route.params['id'];
        return Observable_1.Observable.from(this._appUserDataService.getLoggedInUser());
    };
    return GlobalVariableResolve;
}());
GlobalVariableResolve = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [Services_1.AppUserDataService, router_1.Router])
], GlobalVariableResolve);
exports.GlobalVariableResolve = GlobalVariableResolve;
//# sourceMappingURL=resolve.service.js.map