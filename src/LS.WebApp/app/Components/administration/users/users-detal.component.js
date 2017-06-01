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
    function UsersdetailComponent(_userDataService, _global, toasterService, _constants) {
        var _this = this;
        this._userDataService = _userDataService;
        this._global = _global;
        this._constants = _constants;
        this.loading = true;
        this.hasLoadded = false;
        this.unassignedManager = false;
        this.showSpinner = true;
        alert('contr');
        this.toasterService = toasterService;
        console.log(typeof this._global.loggedInUser);
        if (typeof this._global.loggedInUser == "undefined") {
            this._userDataService.getLoggedInUser().subscribe(function (response) {
                var response = response;
                _this._global.loggedInUser = response.data;
                if (!response.isSuccessful) {
                    alert(response.error.userHelp);
                    return;
                }
            }, function (error) { return _this.msg = error; });
        }
    }
    UsersdetailComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.minToChangeTeam = this._global.minToChangeTeam;
        alert('test');
        console.log(this._global.loggedInUser);
        if (this._global.loggedInUser.role.rank > this._constants.salesTeamManagerRoleRank) {
            this.toasterService.pop('error', 'Permission Error', 'You are not authorized to view this page');
            return;
        }
        else {
            this._userDataService.getAllRoles().subscribe(function (response) {
                var response = response;
                if (!response.isSuccessful) {
                    _this.toasterService.pop('error', 'Error Getting Roles Messages', response.errror.userHelp);
                    return;
                }
                _this.roles = response.data;
                _this.loading = false;
            }, function (error) { return _this.msg = error; });
        } //endelse
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
//# sourceMappingURL=users-detal.component.js.map