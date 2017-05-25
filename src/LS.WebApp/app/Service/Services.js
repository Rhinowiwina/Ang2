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
var http_1 = require("@angular/http");
var Observable_1 = require("rxjs/Observable");
require("rxjs/add/operator/map");
require("rxjs/add/operator/do");
require("rxjs/add/operator/catch");
var CompanyDataService = (function () {
    function CompanyDataService(_http) {
        this._http = _http;
        this.baseUrl = "api/company/";
    }
    CompanyDataService.prototype.getCompany = function (companyId) {
        return this.get(this.baseUrl + "getCompany?companyId=" + companyId);
    };
    //base calls
    CompanyDataService.prototype.get = function (url) {
        return this._http.get(url)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    CompanyDataService.prototype.post = function (url, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.post(url, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    CompanyDataService.prototype.put = function (url, id, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.put(url + id, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    CompanyDataService.prototype.delete = function (url, id) {
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.delete(url + id, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    CompanyDataService.prototype.handleError = function (error) {
        console.error(error);
        return Observable_1.Observable.throw(error.json().error || 'Server error');
    };
    return CompanyDataService;
}());
CompanyDataService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http])
], CompanyDataService);
exports.CompanyDataService = CompanyDataService;
var AppUserDataService = (function () {
    function AppUserDataService(_http) {
        this._http = _http;
        this.baseUrl = 'api/appUser/';
    }
    AppUserDataService.prototype.getLoggedInUser = function () {
        return this.get(this.baseUrl + 'getLoggedInUser');
    };
    //base calls
    AppUserDataService.prototype.get = function (url) {
        return this._http.get(url)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    AppUserDataService.prototype.post = function (url, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.post(url, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    AppUserDataService.prototype.put = function (url, id, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.put(url + id, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    AppUserDataService.prototype.delete = function (url, id) {
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.delete(url + id, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    AppUserDataService.prototype.handleError = function (error) {
        console.error(error);
        return Observable_1.Observable.throw(error.json().error || 'Server error');
    };
    return AppUserDataService;
}());
AppUserDataService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http])
], AppUserDataService);
exports.AppUserDataService = AppUserDataService;
var MessageDataService = (function () {
    function MessageDataService(_http) {
        this._http = _http;
        this.baseUrl = 'api/loginMsg/';
    }
    MessageDataService.prototype.getActiveMessages = function () {
        return this.get(this.baseUrl + 'getActiveMessages');
    };
    //base calls
    MessageDataService.prototype.get = function (url) {
        return this._http.get(url)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    MessageDataService.prototype.post = function (url, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.post(url, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    MessageDataService.prototype.put = function (url, id, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.put(url + id, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    MessageDataService.prototype.delete = function (url, id) {
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.delete(url + id, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    MessageDataService.prototype.handleError = function (error) {
        console.error(error);
        return Observable_1.Observable.throw(error.json().error || 'Server error');
    };
    return MessageDataService;
}());
MessageDataService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http])
], MessageDataService);
exports.MessageDataService = MessageDataService;
//# sourceMappingURL=Services.js.map