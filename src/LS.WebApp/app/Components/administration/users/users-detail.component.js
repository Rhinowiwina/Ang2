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
//https://www.npmjs.com/package/ng2-accordion
var UsersdetailComponent = (function () {
    function UsersdetailComponent(_userDataService, _global, toasterService, _constants) {
        this._userDataService = _userDataService;
        this._global = _global;
        this._constants = _constants;
        this.rowData = [];
        this.loading = true;
        this.hasLoadded = false;
        this.unassignedManager = false;
        this.orgUsers = [];
        this.admins = [];
        this.level1Managers = [];
        this.level2Managers = [];
        this.level3Managers = [];
        this.teamManagers = [];
        this.reps = [];
        this.loadingUsers = [];
        this.erroredRoles = [];
        //test
        this.gridOptions = {};
        this.createCols();
        this.showGrid = true;
        //
        this.toasterService = toasterService;
        this.users = [];
        this.users.push(this.admins);
        this.users.push(this.level1Managers);
        this.users.push(this.level2Managers);
        this.users.push(this.level3Managers);
        this.users.push(this.teamManagers);
        this.users.push(this.reps);
        for (var i = 0; i < 6; i++) {
            this.loadingUsers.push(false);
            this.erroredRoles.push({});
        }
    }
    UsersdetailComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.minToChangeTeam = this._global.minToChangeTeam;
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
                //filter roles the user does not have access to.
                _this.roles = _this.roles.filter(function (role) { return role.rank > _this._global.loggedInUser.role.rank; });
                _this.hasLoadded = true;
            }, function (error) { return _this.msg = error; });
        } //endelse
    };
    UsersdetailComponent.prototype.createCols = function () {
        this.columnDefs = [
            {
                headerName: "Name", headerTooltip: "Active", field: "fullName", minWidth: 70, width: 185,
                cellRenderer: function (params) {
                    if (params.data) {
                        return '<span><a ng-click="modifyUser(' + params.data.id + ')" class="text-link">' + params.data.fullName + '</a></span>';
                    }
                }
            },
            {
                headerName: "Username", field: "userName", headerTooltip: "Username", minWidth: 85, width: 200,
            },
            { headerName: "Promo Code", field: "externalUserID", headerTooltip: "External User ID", minWidth: 85, width: 125, },
            { headerName: "Team Name", field: "team", headerTooltip: "Team Name", minWidth: 85, width: 200, },
            { headerName: "Agent ID", field: "externalDisplayName", headerTooltip: "Agent ID", minWidth: 80, width: 100, },
            { headerName: "Active", field: "isActive", headerTooltip: "Active", minWidth: 65, width: 70, }
        ];
    };
    //
    UsersdetailComponent.prototype.loadUsers = function (rank, index) {
        var _this = this;
        this.loadingUsers[index] = true;
        this._userDataService.getAllUsersUnderLoggedInUserInTree(this._global.loggedInUser.id, rank).subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.loadingUsers[index] = false;
                _this.toasterService.pop('error', 'Error Getting users', response.errror.userHelp);
            }
            _this.users[index] = response.data;
            console.log(_this.users[index]);
            //this.userGridOptions = {
            //	//columnDefs:this.columnDefs,
            //	//angularCompileRows: true,
            //	//rowData: this.users[index],
            //	//enableSorting: true,
            //	//suppressRowClickSelection: true,
            //	//enableColResize: true,
            //	//onGridReady: function (event) { autoSizeAll("u"); sizeToFit('u'); },
            //	//enableFilter: true,
            //	//getRowStyle: function (params) {
            //	//	if (params.node.floating) {
            //	//		return { 'font-weight': 'bold' }
            //	//	}
            //	//},
            //	////isExternalFilterPresent: isExternalFilterPresent,
            //	//doesExternalFilterPass: doesExternalFilterPass
            //};	
            _this.loadingUsers[index] = false;
        }, function (error) { return _this.msg = error; });
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