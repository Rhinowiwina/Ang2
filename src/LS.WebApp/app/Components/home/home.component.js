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
var ng2_bs3_modal_1 = require("ng2-bs3-modal/ng2-bs3-modal");
require("rxjs/add/operator/mergeMap");
var Services_1 = require("../../Service/Services");
var global_1 = require("../../Shared/global");
var router_1 = require("@angular/router");
var angular2_toaster_1 = require("angular2-toaster");
var HomeComponent = (function () {
    function HomeComponent(router, toasterService, _messageDataService, _global) {
        this.router = router;
        this._messageDataService = _messageDataService;
        this._global = _global;
        this.criticalMsg = [];
        this.showModalSpinner = true;
        this.toasterService = toasterService;
    }
    HomeComponent.prototype.ngOnInit = function () {
        this.getMessages();
    };
    HomeComponent.prototype.getMessages = function () {
        var _this = this;
        this._messageDataService.getActiveMessages().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error Getting Login Messages', response.errror.userHelp);
            }
            _this.messages = response.data;
            for (var i = 0; i < _this.messages.length; i++) {
                if (_this.messages[i].msgLevel == 1) {
                    _this.criticalMsg.push(_this.messages[i]);
                }
            }
            if (_this.criticalMsg.length > 0 && !_this._global.criticalMsgRead) {
                _this.modal.open('lg');
                _this.showModalSpinner = false;
            }
        }, function (error) { return _this.msg = error; });
    };
    HomeComponent.prototype.setCriticalMsgRead = function () {
        this.modal.close();
        this._global.criticalMsgRead = true;
    };
    return HomeComponent;
}());
__decorate([
    core_1.ViewChild('modal'),
    __metadata("design:type", ng2_bs3_modal_1.ModalComponent)
], HomeComponent.prototype, "modal", void 0);
HomeComponent = __decorate([
    core_1.Component({
        selector: "app-home",
        templateUrl: "app/Components/home/home.component.html",
        providers: [Services_1.MessageDataService],
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [router_1.Router, angular2_toaster_1.ToasterService, Services_1.MessageDataService, global_1.Global])
], HomeComponent);
exports.HomeComponent = HomeComponent;
//# sourceMappingURL=home.component.js.map