import { Directive, forwardRef } from '@angular/core';
import { NG_VALIDATORS, FormControl } from '@angular/forms';

function validateEmailFactory() {
    return (c: FormControl) => {
       //orignal let EMAIL_REGEXP = /^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i;
        let EMAIL_REGEXP = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        return EMAIL_REGEXP.test(c.value) ? null : {
            validateEmail: {
                valid: false
            }
        };
    };
}

@Directive({
    selector: '[validateEmail][ngModel],[validateEmail][formControl]',
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => EmailValidator), multi: true }
    ]
})
export class EmailValidator {

    validator: Function;

    constructor() {
        this.validator = validateEmailFactory();
    }

    validate(c: FormControl) {
        return this.validator(c);
    }
}
