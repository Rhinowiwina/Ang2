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
require("rxjs/add/operator/mergeMap");
var global_1 = require("../../../Shared/global");
var angular2_toaster_1 = require("angular2-toaster");
var Services_1 = require("../../../Service/Services");
var userBindingModels_1 = require("../../../BindingModels/userBindingModels");
//https://www.npmjs.com/package/ng2-accordion
var modifyUsersComponent = (function () {
    function modifyUsersComponent(route, _userDataService, _salesGroupDataService, _salesTeamDataService, _global, toasterService, _constants) {
        this.route = route;
        this._userDataService = _userDataService;
        this._salesGroupDataService = _salesGroupDataService;
        this._salesTeamDataService = _salesTeamDataService;
        this._global = _global;
        this._constants = _constants;
        this.hasLoaded = false;
        this.roles = [];
        // groups: Array<{group}>=[]
        this.teams = [];
        this.sendingpassword = false;
        this.groups = [];
        this.managerLevel = "";
        this.canBeDeleted = false;
        this.testRole = [{ id: "f1c85c97-2c50-4165-9bb8-248518a109f2", name: "Level 1 Manager", rank: 2 }];
        this.toasterService = toasterService;
    }
    modifyUsersComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.minToChangeTeam = this._global.minToChangeTeam;
        this.sub = this.route.params.subscribe(function (params) {
            _this.userId = params['userId'];
            if (_this.userId) {
                _this.createOrModify = _this._constants.modify;
                _this.getUserToEdit(_this.userId);
            }
            else {
                _this.createOrModify = _this._constants.create;
            }
        });
    };
    modifyUsersComponent.prototype.getUserToEdit = function (userId) {
        var _this = this;
        this.user = new userBindingModels_1.EditUserView();
        this.hasLoaded = false;
        this._userDataService.getUserToEdit(this.userId).subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error getting user to edit.', response.errror.userHelp);
                _this.hasLoaded = true;
            }
            var vuser = response.data;
            // console.log(vuser)
            _this.originalEmail = vuser.email;
            _this.user.id = vuser.id,
                _this.user.comapnyId = vuser.companyId,
                _this.user.userName = vuser.userName,
                _this.user.originalEmail = vuser.email,
                _this.user.firstName = vuser.firstName,
                _this.user.lastName = vuser.lastName,
                _this.user.externalUserID = vuser.externalUserID,
                _this.user.isExternalUserIDActive = vuser.isExternalUserIDActive,
                _this.user.email = vuser.email,
                _this.user.payPalEmail = vuser.payPalEmail,
                _this.user.originalActive = vuser.isActive,
                _this.user.isActive = vuser.isActive,
                _this.user.additionalDataNeeded = vuser.additionalDataNeeded,
                _this.user.permissionsAllowTpivBypass = vuser.permissionsAllowTpivBypass,
                _this.user.permissionsLifelineNlad = vuser.permissionsLifelineNlad,
                _this.user.permissionsLifelineCA = vuser.permissionsLifelineCA,
                _this.user.permissionsLifelineTX = vuser.permissionsLifelineTX,
                _this.user.permissionsAccountOrder = vuser.permissionsAccountOrder,
                _this.user.userCommission = vuser.userCommission,
                _this.user.role = vuser.role;
            _this.user.originalRoleName = vuser.role.name,
                _this.user.team = vuser.salesTeam;
            _this.user.rowVersion = vuser.rowVersion;
            _this.hasLoaded = true;
            if (vuser.role.rank <= _this._global.loggedInUser.role.rank) {
                _this.toasterService.pop('error', 'Permission Error', "You do not have permission to access this user's information.");
                return;
            }
            console.log(_this.user);
            _this.canBeDeleted = vuser.canBeDeleted && _this._global.loggedInUser.role.rank <= _this._constants.administratorRoleRank;
            _this.showResetPasswordButton = true;
            _this.getTeams();
            _this.setRoleFilter();
        }, function (error) { return _this.msg = error; });
    };
    modifyUsersComponent.prototype.getTeams = function () {
        var _this = this;
        this._salesTeamDataService.getSalesTeamsForSelection().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error Sales Teams', response.errror.userHelp);
                return;
            }
            var vTeam = response.data;
            var editedTeam = [];
            vTeam.forEach(function (team) {
                editedTeam.push({
                    id: team.id,
                    name: team.name,
                    externalDisplayName: team.externalDisplayName,
                    isActive: team.isActive,
                    displayText: ""
                });
            });
            _this.teams = editedTeam;
        }, function (error) { return _this.msg = error; });
        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                console.log(response);
                _this.toasterService.pop('error', 'Error Groups', response.error.userHelp);
                return;
            }
            var self = _this;
            var lev1Groups = response.data;
            lev1Groups.forEach(function (lev1group) {
                self.groups.push({
                    name: lev1group.name,
                    id: lev1group.id,
                    level: "1"
                });
                var lev2groups = lev1group.childSalesGroups;
                lev2groups.forEach(function (lev2) {
                    self.groups.push({
                        name: lev2.name,
                        id: lev2.id,
                        level: "2"
                    });
                    var lev3groups = lev2.childSalesGroups;
                    lev3groups.forEach(function (lev3) {
                        self.groups.push({
                            name: lev3.name,
                            id: lev3.id,
                            level: "3"
                        });
                    });
                });
            });
            var teamsLength = _this.teams.length;
            for (var teamIndex = 0; teamIndex < teamsLength; teamIndex++) {
                var team = _this.teams[teamIndex];
                team.displayText = team.externalDisplayName + " / " + team.name;
                if (!team.isActive) {
                    team.displayText += " (Inactive)";
                }
            }
            console.log(_this.user);
            _this.setTeam();
        }, function (error) { return _this.msg = error; });
    };
    modifyUsersComponent.prototype.setTeam = function () {
        var teamsLength = this.teams.length;
        for (var teamsIndex = 0; teamsIndex < teamsLength; teamsIndex++) {
            if (this.user.team.id == this.teams[teamsIndex].id) {
                this.user.team = this.teams[teamsIndex];
            }
        }
    };
    modifyUsersComponent.prototype.setRoleFilter = function () {
        var _this = this;
        this._userDataService.getAllRoles().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error Getting Roles', response.errror.userHelp);
                return;
            }
            var self = _this;
            var vroles = response.data;
            vroles.forEach(function (role) {
                self.roles.push({ id: role.id, name: role.name, rank: role.rank });
            });
            console.log(_this.roles);
            //filter roles the user does not have access to.
            _this.roles = _this.roles.filter(function (role) { return role.rank > _this._global.loggedInUser.role.rank; });
            _this.hasLoaded = true;
        }, function (error) { return _this.msg = error; });
    };
    modifyUsersComponent.prototype.setGroupFilter = function () {
        var vgroupLevel;
        if (this.user.role.name == "Level 1 Manager") {
            this.managerLevel = "Level1";
            vgroupLevel = "1";
        }
        else if (this.user.role.name == "Level 2 Manager") {
            this.managerLevel = "Level2";
            vgroupLevel = "2";
        }
        else if (this.user.role.name == 'Level 3 Manager') {
            this.managerLevel = "Level3";
            vgroupLevel = "3";
        }
        else {
            this.managerLevel = "";
        }
        var vGroups = this.groups;
        var filteredGroup = [];
        vGroups.forEach(function (group) {
            if (group.level == vgroupLevel) {
                filteredGroup.push(group);
            }
        });
        this.groups = filteredGroup;
    };
    modifyUsersComponent.prototype.setRole = function () {
    };
    return modifyUsersComponent;
}());
modifyUsersComponent = __decorate([
    core_1.Component({
        selector: 'modify-user',
        templateUrl: 'app/Components/administration/users/modifyUsers.html',
        styleUrls: ['../../Content/sass/siteAngular.css'],
    }),
    __metadata("design:paramtypes", [router_1.ActivatedRoute, Services_1.AppUserDataService, Services_1.SalesGroupDataService, Services_1.SalesTeamDataService, global_1.Global, angular2_toaster_1.ToasterService, global_1.Constants])
], modifyUsersComponent);
exports.modifyUsersComponent = modifyUsersComponent;
//# sourceMappingURL=modifyUsers.component.js.map