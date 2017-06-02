"use strict";
var core_1 = require("@angular/core");
var Accordion_1 = require("./Accordion");
var AccordionToggle_1 = require("./AccordionToggle");
var AccordionGroup = (function () {
    function AccordionGroup(accordion, cdr) {
        this.accordion = accordion;
        this.cdr = cdr;
        this.isOpened = false;
        this.onOpen = new core_1.EventEmitter();
        this.onClose = new core_1.EventEmitter();
        this.onToggle = new core_1.EventEmitter();
        this.disabled = false;
    }
    AccordionGroup.prototype.checkAndToggle = function () {
        // if custom toggle element is supplied, then do nothing, custom toggler will take care of it
        if (this.toggler)
            return;
        this.toggle();
    };
    AccordionGroup.prototype.toggle = function () {
        if (this.disabled)
            return;
        var isOpenedBeforeWeChange = this.isOpened;
        if (this.accordion.closeOthers)
            this.accordion.closeAll();
        this.isOpened = !isOpenedBeforeWeChange;
        if (this.isOpened) {
            this.onOpen.emit();
        }
        else {
            this.onClose.emit();
        }
        this.onToggle.emit(this.isOpened);
    };
    AccordionGroup.prototype.openOnInitialization = function () {
        this.isOpened = true;
        this.cdr.detectChanges();
    };
    AccordionGroup.decorators = [
        { type: core_1.Component, args: [{
                    selector: "accordion-group",
                    template: "\n  <div class=\"panel panel-default\" [class.dropup]=\"isOpened\" [class.disabled]=\"disabled\">\n    <div class=\"panel-heading\" role=\"tab\" (click)=\"checkAndToggle()\">\n      <h4 class=\"panel-title\">\n        <a *ngIf=\"heading\" role=\"button\" data-toggle=\"collapse\" [attr.aria-expanded]=\"isOpened\">\n            {{ heading }}\n        </a>\n        <ng-content select=\"accordion-heading\"></ng-content>\n        <div class=\"caret\" [style.display]=\"accordion.showArrows ? '' : 'none'\">\n        </div>\n      </h4>\n    </div>\n    <div *ngIf=\"isOpened\" class=\"panel-collapse collapse in\" role=\"tabpanel\" [attr.aria-labelledby]=\"heading\">\n      <div class=\"panel-body\">\n        <ng-content></ng-content>\n      </div>\n    </div>\n  </div>\n"
                },] },
    ];
    /** @nocollapse */
    AccordionGroup.ctorParameters = function () { return [
        { type: Accordion_1.Accordion, decorators: [{ type: core_1.Host }, { type: core_1.Inject, args: [core_1.forwardRef(function () { return Accordion_1.Accordion; }),] },] },
        { type: core_1.ChangeDetectorRef, },
    ]; };
    AccordionGroup.propDecorators = {
        'heading': [{ type: core_1.Input },],
        'isOpened': [{ type: core_1.Input },],
        'onOpen': [{ type: core_1.Output },],
        'onClose': [{ type: core_1.Output },],
        'onToggle': [{ type: core_1.Output },],
        'toggler': [{ type: core_1.ContentChild, args: [AccordionToggle_1.AccordionToggle,] },],
        'disabled': [{ type: core_1.Input },],
    };
    return AccordionGroup;
}());
exports.AccordionGroup = AccordionGroup;
//# sourceMappingURL=AccordionGroup.js.map