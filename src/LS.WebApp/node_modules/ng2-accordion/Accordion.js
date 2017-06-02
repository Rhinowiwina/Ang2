"use strict";
var core_1 = require("@angular/core");
var AccordionGroup_1 = require("./AccordionGroup");
var Accordion = (function () {
    function Accordion() {
        this.closeOthers = true;
        this.showArrows = false;
        this.expandAll = false;
    }
    Accordion.prototype.ngAfterContentInit = function () {
        var _this = this;
        if (this.expandAll) {
            this.closeOthers = false;
            this.oldGroups = this.groups.toArray();
            this.oldGroups.forEach(function (group) {
                group.openOnInitialization();
            });
            // we subscribe for changes, and if new groups are added we open them automatically
            this.subscription = this.groups.changes.subscribe(function (change) {
                var newGroups = _this.groups.toArray().filter(function (group) {
                    return _this.oldGroups.indexOf(group) === -1;
                });
                newGroups.forEach(function (group) {
                    group.openOnInitialization();
                });
                _this.oldGroups = _this.groups.toArray();
            });
        }
    };
    Accordion.prototype.ngOnDestroy = function () {
        if (this.subscription)
            this.subscription.unsubscribe();
    };
    Accordion.prototype.closeAll = function () {
        this.groups.toArray().forEach(function (group) {
            group.isOpened = false;
        });
    };
    Accordion.decorators = [
        { type: core_1.Component, args: [{
                    selector: "accordion",
                    template: "\n<div class=\"panel-group\" role=\"tablist\" aria-multiselectable=\"true\">\n    <ng-content></ng-content>\n</div>\n"
                },] },
    ];
    /** @nocollapse */
    Accordion.ctorParameters = function () { return []; };
    Accordion.propDecorators = {
        'closeOthers': [{ type: core_1.Input },],
        'showArrows': [{ type: core_1.Input },],
        'expandAll': [{ type: core_1.Input },],
        'groups': [{ type: core_1.ContentChildren, args: [core_1.forwardRef(function () { return AccordionGroup_1.AccordionGroup; }),] },],
    };
    return Accordion;
}());
exports.Accordion = Accordion;
//# sourceMappingURL=Accordion.js.map