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
var ngx_bootstrap_1 = require("ngx-bootstrap");
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
        this.userGridOptions = {};
        this.createCols();
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
                    _this.toasterService.pop('error', 'Error Getting Roles', response.errror.userHelp);
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
                        return '<span><a (click)="modifyUser(' + params.data.id + ')" class="text-link">' + params.data.fullName + '</a></span>';
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
        var self = this; //not sure why but must put key work this in variable to use in function call
        this.loadingUsers[index] = true;
        this._userDataService.getAllUsersUnderLoggedInUserInTree(this._global.loggedInUser.id, rank).subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.loadingUsers[index] = false;
                _this.toasterService.pop('error', 'Error Getting Users', response.errror.userHelp);
            }
            _this.users[index] = response.data;
            _this.loadingUsers[index] = false;
            _this.userGridOptions = {
                columnDefs: _this.columnDefs,
                //angularCompileRows: true, not used in version2
                rowData: _this.users[index],
                enableColResize: true,
                enableSorting: true,
                enableFilter: true,
                rowHeight: 22,
                suppressRowClickSelection: true,
                onGridReady: function (event) {
                    self.sizeToFit('u');
                    self.autoSizeAll("u");
                },
                getRowStyle: function (params) {
                    if (params.node.floating) {
                        return { 'font-weight': 'bold' };
                    }
                },
            };
        }, function (error) { return _this.msg = error; });
    };
    //	function isExternalFilterPresent() {
    //	return $scope.showInactiveUsers != true;
    //}
    //function doesExternalFilterPass(node) {
    //	if ($scope.showInactiveUsers) { //This part never actually runs because "false" is returned from isExternalFilterPresent function, which means this function is not ran at all (and all rows are shown)
    //		return node.data.isActive == true || node.data.isActive == false;
    //	} else {
    //		return node.data.isActive == true;
    //	}
    //	 }
    UsersdetailComponent.prototype.sizeToFit = function (form) {
        if (form == 'u') {
            this.userGridOptions.api.sizeColumnsToFit();
        }
        else {
            this.userGridOptions.api.sizeColumnsToFit();
        }
    };
    UsersdetailComponent.prototype.autoSizeAll = function (form) {
        if (form == 'u') {
            var allColumnIds = [];
            this.columnDefs.forEach(function (columnDef) {
                allColumnIds.push(columnDef.field);
            });
            this.userGridOptions.columnApi.autoSizeColumns(allColumnIds);
        }
    };
    UsersdetailComponent.prototype.onBtExport = function (form) {
        var exportName;
        if (form == 'u') {
            var params = {
                allColumns: true,
                fileName: exportName,
            };
            this.userGridOptions.api.exportDataAsCsv(params);
        }
    };
    // externalFilterChanged(newValue) {
    //	$scope.userGridOptions.api.onFilterChanged();
    //}
    //filterUsers(index) {
    //	var newUserArray = [];
    //	$scope.orgUsers[index].forEach(function (user) {
    //		if ($scope.showInactiveUsers) {
    //			newUserArray.push(user);
    //		} else {
    //			if (user.isActive) {
    //				newUserArray.push(user);
    //			}
    //		}
    //	});
    //	if (typeof $scope.userGridOptions.api !== 'undefined') {
    //		$scope.userGridOptions.api.setRowData(newUserArray);
    //	} else {
    //		$scope.userGridOptions.rowData = newUserArray;
    //	}
    //}
    UsersdetailComponent.prototype.closeModal = function (vmodal) {
        this.reportModal.hide();
    };
    UsersdetailComponent.prototype.openModal = function (vmodal) {
        this.reportModal.show();
    };
    return UsersdetailComponent;
}());
__decorate([
    core_1.ViewChild('reportModal'),
    __metadata("design:type", ngx_bootstrap_1.ModalDirective)
], UsersdetailComponent.prototype, "reportModal", void 0);
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