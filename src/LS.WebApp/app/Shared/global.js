"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var core_1 = require("@angular/core");
var Global = (function () {
    function Global() {
        this._criticalMsgRead = false;
    }
    Object.defineProperty(Global.prototype, "minToChangeTeam", {
        get: function () {
            return this._minToChangeTeam;
        },
        set: function (val) {
            this._minToChangeTeam = val;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Global.prototype, "loggedInUser", {
        get: function () {
            return this._loggedInUser;
        },
        set: function (val) {
            this._loggedInUser = val;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Global.prototype, "criticalMsgRead", {
        get: function () {
            return this._criticalMsgRead;
        },
        set: function (val) {
            this._criticalMsgRead = val;
        },
        enumerable: true,
        configurable: true
    });
    return Global;
}());
Global = __decorate([
    core_1.Injectable()
], Global);
exports.Global = Global;
var Constants = (function () {
    function Constants() {
    }
    Object.defineProperty(Constants.prototype, "superadministratorRoleName", {
        get: function () {
            return "Super Administrator";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "superadministratorRoleRank", {
        get: function () {
            return 0;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "administratorRoleName", {
        get: function () {
            return "Administrator";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "administratorRoleRank", {
        get: function () {
            return 1;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "level1ManagerRoleName", {
        get: function () {
            return "Level 1 Manager";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "level1ManagerRoleRank", {
        get: function () {
            return 2;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "level2ManagerRoleName", {
        get: function () {
            return "Level 2 Manager";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "level2ManagerRoleRank", {
        get: function () {
            return 3;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "level3ManagerRoleName", {
        get: function () {
            return "Level 3 Manager";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "level3ManagerRoleRank", {
        get: function () {
            return 4;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "salesTeamManagerRoleName", {
        get: function () {
            return "Sales Team Manager";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "salesTeamManagerRoleRank", {
        get: function () {
            return 5;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "salesRepRoleName", {
        get: function () {
            return "Sales Rep";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Constants.prototype, "salesRepRoleRank", {
        get: function () {
            return 6;
        },
        enumerable: true,
        configurable: true
    });
    return Constants;
}());
Constants = __decorate([
    core_1.Injectable()
], Constants);
exports.Constants = Constants;
//# sourceMappingURL=global.js.map