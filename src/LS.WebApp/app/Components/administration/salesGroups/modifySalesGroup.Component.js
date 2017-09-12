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
var Rx_1 = require("rxjs/Rx");
var Services_1 = require("../../../Service/Services");
require("rxjs/add/operator/mergeMap");
var global_1 = require("../../../Shared/global");
var angular2_toaster_1 = require("angular2-toaster");
var userBindingModels_1 = require("../../../BindingModels/userBindingModels");
var Services_2 = require("../../../Service/Services");
//https://www.npmjs.com/package/ng2-accordion
var ModifySalesGroupComponent = (function () {
    function ModifySalesGroupComponent(router, _appUserDataService, route, _global, _salesGroupDataService, toasterService, _constants) {
        this.router = router;
        this._appUserDataService = _appUserDataService;
        this.route = route;
        this._global = _global;
        this._salesGroupDataService = _salesGroupDataService;
        this.toasterService = toasterService;
        this._constants = _constants;
        this.hasLoaded = false;
        this.allSalesGroups = [];
    }
    ModifySalesGroupComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.allManagers = new userBindingModels_1.GroupsOfManagers();
        this.sub = this.route.queryParams.subscribe(function (params) {
            _this.salesGroupId = params['groupid'];
            _this.paramLevel = params['level'];
            _this.getManagers();
            if (_this.salesGroupId) {
                _this.createOrModify = _this._constants.modify;
                _this.currentLevel = _this.paramLevel;
                _this.getSalesGroupToEdit(_this.salesGroupId);
            }
            else {
                _this.createOrModify = _this._constants.create;
                _this.salesGroupCreate();
            }
        });
    };
    ModifySalesGroupComponent.prototype.ngOnDestroy = function () {
        if (this.sub)
            this.sub.unsubscribe();
    };
    ModifySalesGroupComponent.prototype.getSalesGroups = function () {
        var _this = this;
        var self = this;
        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error retrieving groups.', response.error.userHelp);
                _this.hasLoaded = true;
            }
            _this.level1SalesGroups = response.data;
            _this.level1SalesGroups.forEach(function (level1SalesGroup) {
                self.allSalesGroups.push({ id: level1SalesGroup.id, name: level1SalesGroup.name, level: 1, parentGroupName: '' });
                if (level1SalesGroup.childSalesGroups.length > 0) {
                    var level2SalesGroups = level1SalesGroup.childSalesGroups;
                    level2SalesGroups.forEach(function (level2SalesGroup) {
                        self.allSalesGroups.push({ id: level2SalesGroup.id, name: level2SalesGroup.name, level: 2, parentGroupName: level1SalesGroup.name });
                    });
                }
                self.salesGroup.parentGroupOptions = self.allSalesGroups.filter(function (group) { return group.level == self.currentLevel; });
            });
        }, function (error) { return _this.msg = error; });
    };
    ModifySalesGroupComponent.prototype.getManagers = function () {
        var _this = this;
        var self = this;
        this._appUserDataService.getAllSalesGroupManagers().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error retrieving managers.', response.error.userHelp);
                _this.hasLoaded = true;
            }
            _this.allManagers.level1 = [];
            self.allManagers.level2 = [];
            self.allManagers.level3 = [];
            self.allManagers.level1 = response.data.level1Managers;
            self.allManagers.level2 = response.data.level1Managers;
            self.allManagers.level3 = response.data.level1Managers;
            //initialize all val's to false.
            self.allManagers.level1.forEach(function (manager) {
                manager.val = false;
            });
            self.allManagers.level2.forEach(function (manager) {
                manager.val = false;
            });
            self.allManagers.level3.forEach(function (manager) {
                manager.val = false;
            });
        }, function (error) { return _this.msg = error; });
    };
    ModifySalesGroupComponent.prototype.getSalesGroupToEdit = function (saleGroupId) {
        var _this = this;
        var self = this;
        this._salesGroupDataService.getSalesGroup(this.salesGroupId, this.paramLevel).subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error retrieving group.', response.error.userHelp);
                _this.hasLoaded = true;
            }
            var salesGroup = response.data;
            _this.checkDeletability(salesGroup);
            _this.salesGroup = salesGroup;
            _this.salesGroup.level = _this.paramLevel;
            _this.salesGroup.parentSalesGroupLabel = "Level " + (_this.salesGroup.level - 1);
            _this.getSalesGroups();
            if (_this.currentLevel == 1) {
                _this.salesGroup.managerOptions = _this.allManagers.level1;
            }
            else if (_this.currentLevel == 2) {
                _this.salesGroup.managerOptions = _this.allManagers.level2;
            }
            else if (_this.currentLevel == 3) {
                _this.salesGroup.managerOptions = _this.allManagers.level3;
            }
            _this.setManagers().subscribe(function (response) { _this.hasLoaded = true; }, function (error) { return _this.msg = error; });
            // console.log(this.salesGroup)
        }, function (error) { return _this.msg = error; });
    };
    ModifySalesGroupComponent.prototype.salesGroupCreate = function () {
    };
    ModifySalesGroupComponent.prototype.setManagers = function () {
        if (typeof this.salesGroup.managers == 'undefined' || this.salesGroup.managers == null || this.salesGroup.managers.length < 1) {
            return;
        }
        var checkedManagersLength = this.salesGroup.managers.length; // managers that the user has checked
        var managersLength = this.salesGroup.managerOptions.length; // all available managers for current level
        for (var checkedManagersIndex = 0; checkedManagersIndex < checkedManagersLength; checkedManagersIndex++) {
            for (var managersIndex = 0; managersIndex < managersLength; managersIndex++) {
                if (this.salesGroup.managers[checkedManagersIndex].id == this.salesGroup.managerOptions[managersIndex].id) {
                    this.salesGroup.managerOptions[managersIndex].val = true;
                }
            }
        }
        return Rx_1.Observable.of(true);
    };
    ModifySalesGroupComponent.prototype.checkDeletability = function (salesGroup) {
        if (this.currentLevel == this._constants.salesGroupLevel3) {
            this.canBeDeleted = salesGroup.salesTeams.length == 0 && salesGroup.managers.length == 0;
            return;
        }
        this.canBeDeleted = salesGroup.childSalesGroups.length == 0 && salesGroup.managers.length == 0;
    };
    ModifySalesGroupComponent.prototype.submitSalesGroup = function (groupForm, createOrModify, salesGroupLevel) {
        var _this = this;
        this.submitButtonDisabled = true;
        console.log(this.salesGroup.managers);
        console.log(this.salesGroup.managerOptions);
        this.salesGroup.managers = this.checkForManagers();
        console.log(this.salesGroup.managers);
        this.salesGroup.isDeleted = false;
        if (groupForm.valid) {
            this._salesGroupDataService.submitSalesGroupForAddOrEdit(this.salesGroup, this.currentLevel, this.createOrModify).subscribe(function (response) {
                var response = response;
                if (!response.isSuccessful) {
                    _this.toasterService.pop('error', 'Error saving group.', response.error.userHelp);
                    _this.hasLoaded = true;
                }
            }, function (error) { return _this.msg = error; });
        }
    };
    ModifySalesGroupComponent.prototype.checkForManagers = function () {
        var selectedManagers = [];
        this.salesGroup.managerOptions.forEach(function (manager) {
            //console.log(manager)
            if (manager.val = true) {
                selectedManagers.push(manager);
            }
        });
        console.log(selectedManagers);
        return selectedManagers;
    };
    return ModifySalesGroupComponent;
}());
ModifySalesGroupComponent = __decorate([
    core_1.Component({
        selector: 'modifySales-groups',
        templateUrl: 'app/Components/administration/salesGroups/modifySalesGroups.html',
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [router_1.Router, Services_2.AppUserDataService, router_1.ActivatedRoute, global_1.Global, Services_1.SalesGroupDataService, angular2_toaster_1.ToasterService, global_1.Constants])
], ModifySalesGroupComponent);
exports.ModifySalesGroupComponent = ModifySalesGroupComponent;
//# sourceMappingURL=modifySalesGroup.Component.js.map