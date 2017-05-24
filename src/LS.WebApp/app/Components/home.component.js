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
require("rxjs/add/operator/mergeMap");
var Services_1 = require("../Service/Services");
var global_1 = require("../Shared/global");
var HomeComponent = (function () {
    function HomeComponent(_global, _messageDataService) {
        this._global = _global;
        this._messageDataService = _messageDataService;
    }
    HomeComponent.prototype.ngOnInit = function () {
        this.getMessages();
    };
    HomeComponent.prototype.getMessages = function () {
        var _this = this;
        this._messageDataService.getActiveMessages().subscribe(function (messages) {
            _this.messages = messages.data;
            console.log(_this.messages);
        }, function (error) { return _this.msg = error; });
    };
    return HomeComponent;
}());
HomeComponent = __decorate([
    core_1.Component({
        templateUrl: "app/Components/home.component.html",
        providers: [Services_1.MessageDataService],
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [global_1.Global, Services_1.MessageDataService])
], HomeComponent);
exports.HomeComponent = HomeComponent;
//# sourceMappingURL=home.component.js.map