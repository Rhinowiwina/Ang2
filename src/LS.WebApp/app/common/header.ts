import { Component,ViewEncapsulation } from '@angular/core';

@Component({
	selector: 'app-header',
	encapsulation: ViewEncapsulation.None,
	
 templateUrl: 'app/common/header.html',

})
export class HeaderComponent {
	constructor() {

	}
	  Test:{ Name: string } = {Name:"NewTest"};
	
}