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
var angular2_toaster_1 = require("angular2-toaster");
var Services_1 = require("../../../Service/Services");
var global_2 = require("../../../Shared/global");
//
var UserComponent = (function () {
    function UserComponent(_userDataService, _global, toasterService, _constants) {
        this._userDataService = _userDataService;
        this._global = _global;
        this._constants = _constants;
        this.loading = true;
        this.hasLoadded = false;
        this.unassignedManager = false;
        alert('contr');
        this.toasterService = toasterService;
    }
    UserComponent.prototype.ngOnInit = function () {
        this.loading = false;
        //if (typeof this._global.loggedInUser == "undefined") {
        //	this._userDataService.getLoggedInUser().subscribe(response => {
        //		var response = response;
        //		this._global.loggedInUser = response.data;
        //		this.loading = false;
        //		if (!response.isSuccessful) {
        //			alert(response.error.userHelp)
        //			return
        //		}
        //	}, error => this.msg = <any>error);
        //} else { }
        //if (this._global.loggedInUser.role.rank > this._constants.salesTeamManagerRoleRank) {
        //	this.toasterService.pop('error', 'Permission Error', 'You are not authorized to view this page')
        //	return
        //} else {
        //	this._userDataService.getAllRoles().subscribe
        //		(response => {
        //			var response = response;
        //			if (!response.isSuccessful) {
        //				this.toasterService.pop('error', 'Error Getting Roles Messages', response.errror.userHelp);
        //				return
        //			}
        //			this.roles = response.data;
        //			this.loading = false;
        //		}, error => this.msg = <any>error)
        //}//endelse
    };
    return UserComponent;
}());
UserComponent = __decorate([
    core_1.Component({
        template: "<div *ngIf=\"_global.loggedInUser && !loading\" >\n\t<user-detail > </user-detail>\n      </div>\n\t<img src= \"/Content/img/spiffygif_30x30.gif\" style= \"text-align: center\" [hidden] = \"!loading\"/>\n\t\t",
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [Services_1.AppUserDataService, global_1.Global, angular2_toaster_1.ToasterService, global_2.Constants])
], UserComponent);
exports.UserComponent = UserComponent;
//# sourceMappingURL=users.component.js.map