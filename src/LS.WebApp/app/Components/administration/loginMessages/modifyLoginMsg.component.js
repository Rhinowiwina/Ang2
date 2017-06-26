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
var common_1 = require("@angular/common");
require("rxjs/add/operator/mergeMap");
var global_1 = require("../../../Shared/global");
var angular2_toaster_1 = require("angular2-toaster");
var messageBindingModels_1 = require("../../../BindingModels/messageBindingModels");
var Services_1 = require("../../../Service/Services");
var global_2 = require("../../../Shared/global");
//
var ModifyLoginMsgComponent = (function () {
    function ModifyLoginMsgComponent(router, _messageDataService, _global, toasterService, _constants, datePipe, route) {
        this.router = router;
        this._messageDataService = _messageDataService;
        this._global = _global;
        this._constants = _constants;
        this.datePipe = datePipe;
        this.route = route;
        this.loading = true;
        this.hasLoadded = false;
        this.model = { date: { year: 0, month: 0, day: 0 } };
        this.levels = [{ id: 1, name: 'Critical' }, { id: 2, name: 'Important' }, { id: 3, name: 'Informational' }];
        this.toasterService = toasterService;
    }
    ModifyLoginMsgComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.sub = this.route.params.subscribe(function (params) {
            _this.messageId = params['messageId'];
            if (_this.messageId) {
                _this.createOrModify = _this._constants.modify;
                _this.getMessageToEdit(_this.messageId);
            }
            else {
                _this.createNewMsg();
            }
        });
    };
    ModifyLoginMsgComponent.prototype.ngOnDestroy = function () {
        this.sub.unsubscribe();
    };
    ModifyLoginMsgComponent.prototype.getMessageToEdit = function (messageId) {
        var _this = this;
        this.loading = true;
        this._messageDataService.getMsgToEdit(messageId).subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.loading = false;
                _this.toasterService.pop('error', 'Error Getting Login Message.', response.errror.userHelp);
                _this.loading = false;
            }
            _this.message = response.data;
            _this.loading = false;
        }, function (error) { return _this.msg = error; });
    };
    ModifyLoginMsgComponent.prototype.createNewMsg = function () {
        this.message = new messageBindingModels_1.message();
        this.createOrModify = this._constants.create;
        this.message.active = true;
        this.message.id = "";
        this.message.title = "";
        this.message.msg = "";
        this.message.beginDate = null;
        this.message.expirationDate = null;
        this.message.msgLevel = null;
        this.loading = false;
    };
    ModifyLoginMsgComponent.prototype.submitMessage = function (form) {
        var _this = this;
        if (form.valid) {
            this.loading = true;
            this._messageDataService.submittMessageForAddOrEdit(this.createOrModify, this.message).subscribe(function (response) {
                var response = response;
                if (!response.isSuccessful) {
                    _this.loading = false;
                    _this.toasterService.pop('error', 'Error Updating Login Message.', response.errror.userHelp);
                    _this.loading = false;
                }
                if (_this.createOrModify = _this._constants.create) {
                    var returnMsg = "Successfully added message.";
                }
                else {
                    var returnMsg = "Successfully edited message.";
                }
                _this.toasterService.pop('success', returnMsg);
                _this.loading = false;
                _this.router.navigate(["loginMsg"]);
            }, function (error) { return _this.msg = error; });
        }
    };
    return ModifyLoginMsgComponent;
}());
ModifyLoginMsgComponent = __decorate([
    core_1.Component({
        templateUrl: '../../../app/components/administration/loginMessages/modifyLoginMsg.html',
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [router_1.Router, Services_1.MessageDataService, global_1.Global, angular2_toaster_1.ToasterService, global_2.Constants, common_1.DatePipe, router_1.ActivatedRoute])
], ModifyLoginMsgComponent);
exports.ModifyLoginMsgComponent = ModifyLoginMsgComponent;
//# sourceMappingURL=modifyLoginMsg.component.js.map