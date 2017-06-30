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
var Services_1 = require("../../../Service/Services");
require("rxjs/add/operator/mergeMap");
var global_1 = require("../../../Shared/global");
var angular2_toaster_1 = require("angular2-toaster");
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
    }
    ModifySalesGroupComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.getSalesGroups();
        this.getManagers();
        this.sub = this.route.params.subscribe(function (params) {
            _this.salesGroupId = params['id'];
            _this.paramLevel = params['level'];
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
    ModifySalesGroupComponent.prototype.getSalesGroups = function () {
        var _this = this;
        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error retrieving groups.', response.error.userHelp);
                _this.hasLoaded = true;
            }
            _this.level1SalesGroups = response.data;
            _this.level1SalesGroups.forEach(function (level1SalesGroup) {
                var _this = this;
                this.allSalesGroups.push({ id: level1SalesGroup.id, name: level1SalesGroup.name, level: 1, parentGroupName: '' });
                if (level1SalesGroup.childSalesGroups.length > 0) {
                    var level2SalesGroups = level1SalesGroup.childSalesGroups;
                    level2SalesGroups.forEach(function (level2SalesGroup) {
                        this.allSalesGroups.push({ id: level2SalesGroup.id, name: level2SalesGroup.name, level: 2, parentGroupName: level1SalesGroup.name });
                    });
                }
                this.salesGroup.parentGroupOptions = this.allSalesGroup.level.filter(function (group) { return group.level == _this.currentLevel; });
            });
        }, function (error) { return _this.msg = error; });
    };
    ModifySalesGroupComponent.prototype.getManagers = function () {
        var _this = this;
        this._appUserDataService.getAllSalesGroupManagers().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error retrieving managers.', response.error.userHelp);
                _this.hasLoaded = true;
            }
            var level1Managers = response.data.level1Managers;
            level1Managers.forEach(function (level1Manager) {
                this.allManagers.push(level1Manager);
            });
            var level2Managers = response.data.level2Managers;
            level2Managers.forEach(function (level2Manager) {
                this.allManagers.push(level2Manager);
            });
            var level3Managers = response.data.level3Managers;
            level3Managers.forEach(function (level3Manager) {
                this.allManagers.push(level3Manager);
            });
        }, function (error) { return _this.msg = error; });
    };
    ModifySalesGroupComponent.prototype.getSalesGroupToEdit = function (saleGroupId) {
        var _this = this;
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
            _this.salesGroup.managerOptions = _this.allManagers.filter(function (manager) { return manager.level = _this.currentLevel; });
        }, function (error) { return _this.msg = error; });
    };
    ModifySalesGroupComponent.prototype.salesGroupCreate = function () {
    };
    ModifySalesGroupComponent.prototype.checkDeletability = function (salesGroup) {
        if (this.currentLevel == this._constants.salesGroupLevel3) {
            this.canBeDeleted = salesGroup.salesTeams.length == 0 && salesGroup.managers.length == 0;
            return;
        }
        this.canBeDeleted = salesGroup.childSalesGroups.length == 0 && salesGroup.managers.length == 0;
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
//# sourceMappingURL=modifySalesGroupComponent.js.map