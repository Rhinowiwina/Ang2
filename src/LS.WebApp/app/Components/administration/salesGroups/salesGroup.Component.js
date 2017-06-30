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
var AccordionGroup_1 = require("../../../Shared/Accordion/AccordionGroup");
//https://www.npmjs.com/package/ng2-accordion
var SalesGroupComponent = (function () {
    function SalesGroupComponent(router, _global, _salesGroupDataService, toasterService, _constants) {
        this.router = router;
        this._global = _global;
        this._salesGroupDataService = _salesGroupDataService;
        this.toasterService = toasterService;
        this._constants = _constants;
        this.hasLoaded = false;
        this.showLevel1Manager = [];
        this.showLevel2Manager = [];
        this.showLevel3Manager = [];
        this.showLevel3Team = [];
        this.unassingedManager = false;
    }
    SalesGroupComponent.prototype.ngOnInit = function () {
        this.getAndFormatSalesGroups();
    };
    SalesGroupComponent.prototype.getAndFormatSalesGroups = function () {
        var _this = this;
        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.toasterService.pop('error', 'Error retrieving groups.', response.error.userHelp);
                _this.hasLoaded = true;
            }
            _this.salesGroups = response.data;
            if (_this.salesGroups.length < 1) {
                _this.unassingedManager = true;
            }
            _this.hasLoaded = true;
        }, function (error) { return _this.msg = error; });
    };
    SalesGroupComponent.prototype.showManagers = function (index, groupId, level, event, isOpen) {
        if (isOpen && event) {
            event.preventDefault();
            event.stopPropagation();
        }
        if (level == this._constants.salesGroupLevel1) {
            this.showMgrsInLevel1Group(groupId);
            this.showLevel1Manager[index] = !this.showLevel1Manager[index];
        }
        else if (level == this._constants.salesGroupLevel2) {
            this.showMgrsInLevel2Group(groupId);
            this.showLevel2Manager[index] = !this.showLevel2Manager[index];
        }
        else if (level == this._constants.salesGroupLevel3) {
            this.showLevel3Manager[index] = this.showLevel3Manager[index];
        }
        return false;
    };
    SalesGroupComponent.prototype.showMgrsInLevel1Group = function (salesGroupLevel1Id) {
        var _this = this;
        if (typeof this.showLevel1Manager[salesGroupLevel1Id] == 'undefined') {
            this._salesGroupDataService.getGroupManagers(salesGroupLevel1Id, 1).subscribe(function (response) {
                var response = response;
                if (!response.isSuccessful) {
                    _this.toasterService.pop('error', 'Error retrieving level1 managers.', response.error.userHelp);
                    _this.hasLoaded = true;
                }
                _this.salesGroups.forEach(function (Lev1Group) {
                    if (Lev1Group.id == salesGroupLevel1Id) {
                        Lev1Group.managers = response.data;
                    }
                });
            }, function (error) { return _this.msg = error; });
        }
        this.showLevel1Manager[salesGroupLevel1Id] = !this.showLevel1Manager[salesGroupLevel1Id];
    };
    SalesGroupComponent.prototype.showMgrsInLevel2Group = function (salesGroupLevel2Id) {
        var _this = this;
        if (typeof this.showLevel2Manager[salesGroupLevel2Id] == 'undefined') {
            this._salesGroupDataService.getGroupManagers(salesGroupLevel2Id, 2).subscribe(function (response) {
                var response = response;
                if (!response.isSuccessful) {
                    _this.toasterService.pop('error', 'Error retrieving level2 managers.', response.error.userHelp);
                    _this.hasLoaded = true;
                }
                _this.salesGroups.forEach(function (Lev1Group) {
                    Lev1Group.childSalesGroups.forEach(function (Lev2Group) {
                        if (Lev2Group.id == salesGroupLevel2Id) {
                            Lev2Group.managers = response.data;
                        }
                    });
                });
            }, function (error) { return _this.msg = error; });
        }
        this.showLevel2Manager[salesGroupLevel2Id] = !this.showLevel2Manager[salesGroupLevel2Id];
    };
    SalesGroupComponent.prototype.showMgrsInLevel3Group = function (salesGroupLevel3Id) {
        var _this = this;
        if (typeof this.showLevel2Manager[salesGroupLevel3Id] == 'undefined') {
            this._salesGroupDataService.getGroupManagers(salesGroupLevel3Id, 3).subscribe(function (response) {
                var response = response;
                if (!response.isSuccessful) {
                    _this.toasterService.pop('error', 'Error retrieving level3 managers.', response.error.userHelp);
                    _this.hasLoaded = true;
                }
                _this.salesGroups.forEach(function (Lev1Group) {
                    Lev1Group.childSalesGroups.forEach(function (Lev2Group) {
                        Lev2Group.childSalesGroups.forEach(function (Lev3Group) {
                            if (Lev3Group.id == salesGroupLevel3Id) {
                                Lev3Group.managers = response.data;
                            }
                        });
                    });
                });
            }, function (error) { return _this.msg = error; });
        }
        this.showLevel3Manager[salesGroupLevel3Id] = !this.showLevel3Manager[salesGroupLevel3Id];
    };
    SalesGroupComponent.prototype.showTeamsInLevel3Group = function (salesGroupLevel3Id) {
        var _this = this;
        if (typeof this.showLevel3Team[salesGroupLevel3Id] == 'undefined') {
            this._salesGroupDataService.getLevel3GroupTeams(salesGroupLevel3Id).subscribe(function (response) {
                var response = response;
                if (!response.isSuccessful) {
                    _this.toasterService.pop('error', 'Error retrieving sales teams.', response.error.userHelp);
                    _this.hasLoaded = true;
                }
                _this.salesGroups.forEach(function (Lev1Group) {
                    Lev1Group.childSalesGroups.forEach(function (Lev2Group) {
                        Lev2Group.childSalesGroups.forEach(function (Lev3Group) {
                            if (Lev3Group.id == salesGroupLevel3Id) {
                                Lev3Group.salesTeams = response.data;
                            }
                        });
                    });
                });
            }, function (error) { return _this.msg = error; });
        }
        this.showLevel3Team[salesGroupLevel3Id] = !this.showLevel3Team[salesGroupLevel3Id];
    };
    SalesGroupComponent.prototype.modifySalesGroup = function (level, groupid, event) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        this.router.navigate(["modifySalesgroup"], { queryParams: { groupid: groupid, level: level } });
        return false;
    };
    return SalesGroupComponent;
}());
__decorate([
    core_1.ViewChild(AccordionGroup_1.AccordionGroup),
    __metadata("design:type", AccordionGroup_1.AccordionGroup)
], SalesGroupComponent.prototype, "accordion", void 0);
SalesGroupComponent = __decorate([
    core_1.Component({
        selector: 'sales-groups',
        templateUrl: 'app/Components/administration/salesGroups/salesGroups.html',
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [router_1.Router, global_1.Global, Services_1.SalesGroupDataService, angular2_toaster_1.ToasterService, global_1.Constants])
], SalesGroupComponent);
exports.SalesGroupComponent = SalesGroupComponent;
//# sourceMappingURL=salesGroup.Component.js.map