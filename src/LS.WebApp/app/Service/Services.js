"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
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
var global_1 = require("../Shared/global");
var BaseService = (function () {
    function BaseService(_http, _constants) {
        this._http = _http;
        this._constants = _constants;
        //_http is sent from inherited class
    }
    //base calls
    BaseService.prototype.get = function (url) {
        return this._http.get(url)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    BaseService.prototype.post = function (url, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.post(url, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    BaseService.prototype.put = function (url, id, model) {
        var body = JSON.stringify(model);
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.put(url + id, body, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    BaseService.prototype.delete = function (url, id) {
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.delete(url + id, options)
            .map(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    BaseService.prototype.handleError = function (error) {
        return Observable_1.Observable.throw(error.json().error || 'Server error');
    };
    return BaseService;
}());
BaseService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http, global_1.Constants])
], BaseService);
exports.BaseService = BaseService;
var CompanyDataService = (function (_super) {
    __extends(CompanyDataService, _super);
    function CompanyDataService(vhttp, constants) {
        var _this = _super.call(this, vhttp, constants) || this;
        _this.vhttp = vhttp;
        _this.constants = constants;
        _this.baseUrl = "api/company/";
        return _this;
    }
    CompanyDataService.prototype.getCompany = function (companyId) {
        return _super.prototype.get.call(this, this.baseUrl + "getCompany?companyId=" + companyId);
    };
    return CompanyDataService;
}(BaseService));
CompanyDataService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http, global_1.Constants])
], CompanyDataService);
exports.CompanyDataService = CompanyDataService;
var AppUserDataService = (function (_super) {
    __extends(AppUserDataService, _super);
    function AppUserDataService(vhttp, constants) {
        var _this = _super.call(this, vhttp, constants) || this;
        _this.vhttp = vhttp;
        _this.constants = constants;
        _this.baseUrl = 'api/appUser/';
        return _this;
    }
    AppUserDataService.prototype.getLoggedInUser = function () {
        return this.get(this.baseUrl + 'getLoggedInUser');
    };
    AppUserDataService.prototype.getAllUsersUnderLoggedInUserInTree = function (UserId, rank) {
        return this.get(this.baseUrl + 'getAllUsersUnderLoggedInUserInTree?UserID=' + UserId + '&Rank=' + rank);
    };
    AppUserDataService.prototype.getAllRoles = function () {
        return this.get(this.baseUrl + "getAllRoles");
    };
    return AppUserDataService;
}(BaseService));
AppUserDataService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http, global_1.Constants])
], AppUserDataService);
exports.AppUserDataService = AppUserDataService;
var MessageDataService = (function (_super) {
    __extends(MessageDataService, _super);
    function MessageDataService(vhttp, constants) {
        var _this = _super.call(this, vhttp, constants) || this;
        _this.vhttp = vhttp;
        _this.constants = constants;
        _this.baseUrl = 'api/loginMsg/';
        return _this;
    }
    MessageDataService.prototype.getActiveMessages = function () {
        return this.get(this.baseUrl + 'getActiveMessages');
    };
    MessageDataService.prototype.getAllMessages = function () {
        return this.get(this.baseUrl + 'getAllMessages');
    };
    MessageDataService.prototype.getMsgToEdit = function (messageId) {
        return this.get(this.baseUrl + 'getMsgToEdit?messageId=' + messageId);
    };
    MessageDataService.prototype.submittMessageForAddOrEdit = function (createOrModify, message) {
        var postData = {
            id: message.id,
            title: message.title,
            msg: message.msg,
            beginDate: message.beginDate,
            expirationDate: message.expirationDate,
            msgLevel: message.msgLevel,
            active: message.active
        };
        if (createOrModify == this.constants.modify) {
            return this.post(this.baseUrl + 'editMessage', postData);
        }
        if (createOrModify == this.constants.create) {
            return this.post(this.baseUrl + 'createMessage', postData);
        }
    };
    return MessageDataService;
}(BaseService));
MessageDataService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http, global_1.Constants])
], MessageDataService);
exports.MessageDataService = MessageDataService;
//# sourceMappingURL=Services.js.map