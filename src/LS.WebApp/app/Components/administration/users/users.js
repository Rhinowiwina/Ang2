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
var global_1 = require("../../../Shared/global");
var Services_1 = require("../../../Service/Services");
var UserComponent = (function () {
    function UserComponent(_userDataService, _global) {
        this._userDataService = _userDataService;
        this._global = _global;
        //criticalMsg: Array<message> = [];
        this.showSpinner = true;
    }
    UserComponent.prototype.ngOnInit = function () {
        //	if (this._global.loggedInUser.Role)
        //}
        //loadUsers(rank:string, index:number) {
        //}
        //getMessages() {
        //	this._messageDataService.getActiveMessages().subscribe(response => {
        //		var response = response;
        //		if (!response.isSuccessful) {
        //			this.toasterService.pop('error', 'Error Getting Login Messages', response.errror.userHelp);
        //		}
        //		this.messages = response.data;
        //		for (let i = 0; i < this.messages.length; i++) {
        //			if (this.messages[i].msgLevel == 1) {
        //				this.criticalMsg.push(this.messages[i])
        //			}
        //		}
        //		//alert(this._global.criticalMsgRead)
        //		if (this.criticalMsg.length > 0) {//&& !this._global.criticalMsgRead
        //			//this.modal.open('lg')
        //			this.showModalSpinner = false;
        //		}
        //	}, error => this.msg = <any>error);
        //}
        //setCriticalMsgRead() {
        //	this.modal.close()
        //	//this._global.criticalMsgRead = true;
        //}
    };
    return UserComponent;
}());
UserComponent = __decorate([
    core_1.Component({
        templateUrl: "app/Components/admin/users/users.html",
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [Services_1.AppUserDataService, global_1.Global])
], UserComponent);
exports.UserComponent = UserComponent;
//# sourceMappingURL=users.js.map