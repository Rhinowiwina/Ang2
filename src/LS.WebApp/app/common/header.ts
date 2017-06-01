import { Component, ViewEncapsulation, Input, Attribute, OnChanges, OnInit } from '@angular/core';
import { EmitterService } from '../Service/emitter.service';
import { CompanyDataService } from '../Service/Services';

import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router, ActivatedRoute, Params } from '@angular/router';
@Component({
	selector: 'app-header',
	encapsulation: ViewEncapsulation.None,
 templateUrl: 'app/common/header.html',

})
export class HeaderComponent implements OnInit {
	primarycolor: string;
	secondarycolor: string;
	logopath: string;
	style:string
	companyLogo: string ="../../Content/img/SpinSolutions.png";
	companyTitle: string ="SpinSolutionsSPA";
	
	msg: string;
	@Input() brandingmodel: { companyLogoUrl: string, name: string };
	@Input() loggedInUser: {};
	constructor() { }
	ngOnInit(): void {
		this.companyLogo = this.brandingmodel.companyLogoUrl;
		this.companyTitle = this.brandingmodel.name;
		}
	
	
	LogOut() {
		alert('logout')
		window.location.href = "/logout?userid=" +"1";}
	
	

	
}