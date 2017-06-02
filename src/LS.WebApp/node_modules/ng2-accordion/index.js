"use strict";
function __export(m) {
    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
}
var core_1 = require("@angular/core");
var common_1 = require("@angular/common");
var AccordionGroup_1 = require("./AccordionGroup");
var Accordion_1 = require("./Accordion");
var AccordionToggle_1 = require("./AccordionToggle");
var AccordionHeading_1 = require("./AccordionHeading");
__export(require("./Accordion"));
__export(require("./AccordionGroup"));
__export(require("./AccordionToggle"));
__export(require("./AccordionHeading"));
var AccordionModule = (function () {
    function AccordionModule() {
    }
    AccordionModule.decorators = [
        { type: core_1.NgModule, args: [{
                    imports: [
                        common_1.CommonModule
                    ],
                    declarations: [
                        Accordion_1.Accordion,
                        AccordionGroup_1.AccordionGroup,
                        AccordionToggle_1.AccordionToggle,
                        AccordionHeading_1.AccordionHeading
                    ],
                    exports: [
                        Accordion_1.Accordion,
                        AccordionGroup_1.AccordionGroup,
                        AccordionToggle_1.AccordionToggle,
                        AccordionHeading_1.AccordionHeading
                    ]
                },] },
    ];
    /** @nocollapse */
    AccordionModule.ctorParameters = function () { return []; };
    return AccordionModule;
}());
exports.AccordionModule = AccordionModule;
//# sourceMappingURL=index.js.map