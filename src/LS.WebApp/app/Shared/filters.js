"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var core_1 = require("@angular/core");
var YesNo = (function () {
    function YesNo() {
    }
    YesNo.prototype.transform = function (data, filter) {
        if (typeof data == 'undefined' || data == null) {
            return "";
        }
        if (data == true) {
            return 'Yes';
        }
        else {
            return 'No';
        }
    };
    return YesNo;
}());
YesNo = __decorate([
    core_1.Pipe({
        name: 'YesNo',
        pure: false
    })
], YesNo);
exports.YesNo = YesNo;
var MsgLevel = (function () {
    function MsgLevel() {
    }
    MsgLevel.prototype.transform = function (input, filter) {
        if (input == 1) {
            return 'Warning';
        }
        else if (input == 2) {
            return 'Important';
        }
        else if (input == 3) {
            return 'Informational';
        }
    };
    return MsgLevel;
}());
MsgLevel = __decorate([
    core_1.Pipe({
        name: 'msgLevel',
        pure: false
    })
], MsgLevel);
exports.MsgLevel = MsgLevel;
//# sourceMappingURL=filters.js.map