import { Component,ViewEncapsulation,Input,Attribute } from '@angular/core';

@Component({
	selector: 'app-header',
	encapsulation: ViewEncapsulation.None,
	
 templateUrl: 'app/common/header.html',

})
export class HeaderComponent {
	@Input()
	test: string;
	@Input() brandingmodel: any;
	constructor() {
		console.log(this.test);
	}
	ngOnInit() {
		console.log(this.brandingmodel);
	}
	

	theval: string = 'red';
	custstyle: string = '.navbar {background - color:' + this.theval + '}'
	public my_Class = 'custstyle';
	
}