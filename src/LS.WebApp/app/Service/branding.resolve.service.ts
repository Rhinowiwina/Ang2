// branding.resolve.service.ts
//http://shermandigital.com/blog/wait-for-data-before-rendering-views-in-angular-2/
//https://blog.thoughtram.io/angular/2016/10/10/resolving-route-data-in-angular-2.html
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Router, Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { AppComponent } from '../app.component';
import { CompanyDataService } from '../Service/Services';

@Injectable()
export class BrandingResolve implements Resolve<any> {
	branding: {CompanyLogoUrl:string};
	msg: string;
	
	constructor(private _brandingService: CompanyDataService, private router: Router, ) { }

	resolve(route: ActivatedRouteSnapshot) {
		let id = +route.params['id'];
	return this._brandingService.get("api/company/getCompany?companyId=65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c")

		
	

	}
}