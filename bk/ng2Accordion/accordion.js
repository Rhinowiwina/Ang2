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
//https://embed.plnkr.co/NzMrJixtwZv0l2ohiZgQ/
var Accordion = (function () {
    function Accordion() {
        this.groups = [];
    }
    Accordion.prototype.addGroup = function (group) {
        this.groups.push(group);
    };
    Accordion.prototype.closeOthers = function (openGroup) {
        this.groups.forEach(function (group) {
            if (group !== openGroup) {
                group.isOpen = false;
            }
        });
    };
    Accordion.prototype.removeGroup = function (group) {
        var index = this.groups.indexOf(group);
        if (index !== -1) {
            this.groups.splice(index, 1);
        }
    };
    return Accordion;
}());
Accordion = __decorate([
    core_1.Component({
        selector: 'accordion',
        template: "\n  <ng-content></ng-content>\n          ",
        host: {
            'class': 'panel-group'
        }
    })
], Accordion);
exports.Accordion = Accordion;
var AccordionGroup = (function () {
    function AccordionGroup(accordion) {
        this.accordion = accordion;
        this._isOpen = false;
        this.headerOpened = new core_1.EventEmitter();
        this.accordion.addGroup(this);
    }
    Object.defineProperty(AccordionGroup.prototype, "isOpen", {
        get: function () {
            return this._isOpen;
        },
        set: function (value) {
            this._isOpen = value;
            if (value) {
                this.accordion.closeOthers(this);
                this.headerOpened.emit();
            }
        },
        enumerable: true,
        configurable: true
    });
    AccordionGroup.prototype.ngOnDestroy = function () {
        this.accordion.removeGroup(this);
    };
    AccordionGroup.prototype.toggleOpen = function (event) {
        event.preventDefault();
        this.isOpen = !this.isOpen;
    };
    return AccordionGroup;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", String)
], AccordionGroup.prototype, "heading", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], AccordionGroup.prototype, "headerOpened", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Boolean),
    __metadata("design:paramtypes", [Boolean])
], AccordionGroup.prototype, "isOpen", null);
AccordionGroup = __decorate([
    core_1.Component({
        selector: 'accordion-group',
        template: " \n                <div  class=\"panel panel-default\" [ngClass]=\"{'panel-open': isOpen}\">\n                 <accordion-head [heading]=\"heading\" (toggled)=toggleOpen($event)> </accordion-head>\n                  <div class=\"panel-collapse\" [hidden]=\"!isOpen\">\n \n                    <div class=\"panel-body\">\n                        <ng-content></ng-content>\n                    </div>\n                  </div>\n                </div>\n\n          ",
    }),
    __metadata("design:paramtypes", [Accordion])
], AccordionGroup);
exports.AccordionGroup = AccordionGroup;
var AccordionHead = (function () {
    function AccordionHead() {
        this._isOpen = false;
        this.toggled = new core_1.EventEmitter();
    }
    AccordionHead.prototype.toggleOpen = function (event) {
        event.preventDefault();
        //this.isOpen = !this.isOpen;
        this.toggled.emit(event);
    };
    return AccordionHead;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", String)
], AccordionHead.prototype, "heading", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", core_1.EventEmitter)
], AccordionHead.prototype, "toggled", void 0);
AccordionHead = __decorate([
    core_1.Component({
        selector: 'accordion-head',
        template: "\n                <div class=\"panel-heading\" (click)=\"toggleOpen($event)\">\n                    <h4 class=\"panel-title\">\n                      <a href tabindex=\"0\">{{heading}}</a>\n                     <button type=\"button\" class=\"btn admin-button text-right\" ng-click=\"showManagers($index, salesGroupLevel1.id, salesGroupConstants.salesGroupLevel1, $event, salesGroupLevel1.open)\">Managers</button>\n                    </h4>\n                     \n                  </div>\n          ",
    })
], AccordionHead);
exports.AccordionHead = AccordionHead;
//# sourceMappingURL=accordion.js.map