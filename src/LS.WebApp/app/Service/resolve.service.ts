// branding.resolve.service.ts
//http://shermandigital.com/blog/wait-for-data-before-rendering-views-in-angular-2/
//https://blog.thoughtram.io/angular/2016/10/10/resolving-route-data-in-angular-2.html
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Router, Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { AppComponent } from '../app.component';
import { CompanyDataService, AppUserDataService } from '../Service/Services';
import { Global } from '../Shared/Global';
import { LoggedInUser } from '../BindingModels/userBindingModels';
@Injectable()
export class LoggedInUserResolve implements Resolve<any> {
	branding: {CompanyLogoUrl:string};
	msg: string;
	
	constructor(private _global: Global, private router: Router, ) { }

	resolve(route: ActivatedRouteSnapshot) {
		let id = +route.params['id'];
	return this._global.loggedInUser
	}
}

@Injectable()
export class GlobalVariableResolve implements Resolve<any> {
	loggedInUser: LoggedInUser;
	msg: string;
	
	constructor(private _appUserDataService: AppUserDataService, private router: Router, ) { alert('tere39')}

	resolve(route: ActivatedRouteSnapshot): Observable<LoggedInUser> {
		let id = +route.params['id'];
		return Observable.from(this._appUserDataService.getLoggedInUser())


			
		}
	
}