"use strict";
var core_1 = require("@angular/core");
var AccordionGroup_1 = require("./AccordionGroup");
var AccordionToggle = (function () {
    function AccordionToggle(accordionGroup) {
        this.accordionGroup = accordionGroup;
    }
    AccordionToggle.prototype.onClick = function () {
        this.accordionGroup.toggle();
    };
    AccordionToggle.decorators = [
        { type: core_1.Component, args: [{
                    selector: "accordion-toggle",
                    template: "<ng-content></ng-content>"
                },] },
    ];
    /** @nocollapse */
    AccordionToggle.ctorParameters = function () { return [
        { type: AccordionGroup_1.AccordionGroup, decorators: [{ type: core_1.Host }, { type: core_1.Inject, args: [core_1.forwardRef(function () { return AccordionGroup_1.AccordionGroup; }),] },] },
    ]; };
    AccordionToggle.propDecorators = {
        'onClick': [{ type: core_1.HostListener, args: ["click",] },],
    };
    return AccordionToggle;
}());
exports.AccordionToggle = AccordionToggle;
//# sourceMappingURL=AccordionToggle.js.map