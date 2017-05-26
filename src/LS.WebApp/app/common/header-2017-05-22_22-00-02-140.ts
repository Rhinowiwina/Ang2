import { Component,ViewEncapsulation,Input,Attribute } from '@angular/core';

@Component({
	selector: 'app-header',
	encapsulation: ViewEncapsulation.None,
	
 templateUrl: 'app/common/header.html',

})
export class HeaderComponent {
	primarycolor: string;
	imageurl: string;
	@Input() brandingmodel: any;
	
	constructor() {
		
	}
	ngOnInit() {
		this.primarycolor = '#' + this.brandingmodel.PrimaryColorHex;
		this.imageurl = '/Content/img/' + this.brandingmodel.CompanyLogoUrl;
		console.log(this.brandingmodel);
	
		
	}
	


	
}