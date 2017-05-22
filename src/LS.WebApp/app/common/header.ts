import { Component,ViewEncapsulation,Input,Attribute } from '@angular/core';

@Component({
	selector: 'app-header',
	encapsulation: ViewEncapsulation.None,
	
 templateUrl: 'app/common/header.html',

})
export class HeaderComponent {
	primarycolor: string;
	secondarycolor: string;
	logopath: string;
	style:string
	imageurl: string;
	@Input() brandingmodel: any;
	
	constructor() {
		
	}
	ngOnInit() {	
		this.primarycolor = '#d9970e'    //'#' + this.brandingmodel.PrimaryColorHex;
		this.logopath = '';
		this.secondarycolor = '';
	    this.imageurl = '/Content/img/' + this.brandingmodel.CompanyLogoUrl;
		console.log(this.brandingmodel);	
	}
	LogOut() {
		alert('logout')
		window.location.href = "/logout?userid=" +"1";}
	


	
}