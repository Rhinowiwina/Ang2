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
var UsersdetailComponent = (function () {
    //loginmsg: {};
    //hasLoadded: boolean = false;
    //minToChangeTeam: number;
    //unassignedManager: boolean = false;
    //roles: {}
    //users: {
    //	admins: Array<UserView>,
    //	level1Managers: Array<UserView>,
    //	level2Managers: Array<UserView>, 
    //	level3Managers: Array<UserView>,
    //	teamManagers: Array<UserView>, 
    //	reps: Array<UserView>
    //};
    //private toasterService: ToasterService;
    function UsersdetailComponent(_userDataService, _global, toasterService, _constants) {
        //this.toasterService = toasterService;
        this._userDataService = _userDataService;
        this._global = _global;
        this._constants = _constants;
        this.loading = true;
    }
    UsersdetailComponent.prototype.ngOnInit = function () {
        //this.minToChangeTeam = this._global.minToChangeTeam;
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
    return UsersdetailComponent;
}());
UsersdetailComponent = __decorate([
    core_1.Component({
        selector: 'user-detail',
        templateUrl: 'app/Components/administration/users/users-detail.html',
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [Services_1.AppUserDataService, global_1.Global, angular2_toaster_1.ToasterService, global_2.Constants])
], UsersdetailComponent);
exports.UsersdetailComponent = UsersdetailComponent;
//# sourceMappingURL=users-detail.component.js.map