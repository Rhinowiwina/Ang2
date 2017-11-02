//Toast Selector is put here and accessible by all components. Default config is set in toastr-config.js
//https://github.com/Stabzs/Angular2-Toaster/blob/master/README.md
//error: 'icon-error',
//info: 'icon-info',
//wait: 'icon-wait',
//success: 'icon-success',
//warning: 'icon-warning'
import { Component, OnInit, ViewChild, Input, OnChanges, ViewEncapsulation } from "@angular/core"
import { CompanyDataService } from './Service/Services';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType  } from 'angular2-toaster';

import { AppUserDataService } from './Service/Services';
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { EmitterService } from './Service/emitter.service';
import { Global } from './Shared/global';
import { HeaderComponent } from "./common/header"
import { BrandingComponent } from './common/branding'
import { LoggedInUser } from './BindingModels/userBindingModels';
import { Company} from './BindingModels/companyBindingModels';
@Component({

	selector: "my-app",	
	template: 
`	 <div *ngIf="branding && loggedInUser" >
		      <app-header [brandingmodel]="branding" [loggedInUser]="loggedInUser"> 
              </app-header></div>
     <toaster-container [toasterconfig]="toasterconfig"></toaster-container>
 
            <div class='container container-content panel panel-default'>
                <router-outlet></router-outlet>
            </div>
			`,
	styleUrls:['app/app.component.css'],


	encapsulation: ViewEncapsulation.None,
	})

export class AppComponent implements OnInit {
	private toasterService: ToasterService;
	public toasterconfig: ToasterConfig =
	new ToasterConfig({
	     limit:4,
		positionClass: 'toast-bottom-right',
		showCloseButton: true,
		tapToDismiss: false,
		bodyOutputType: BodyOutputType.TrustedHtml,
		//closeHtml: '<button>Close</button>'
		//timeout:5000
	});
	branding:Company;
    loggedInUser: LoggedInUser;
    test:string='test'
	msg: string;
	constructor(toasterService: ToasterService, private _companyDataService: CompanyDataService, private _appUserDataService: AppUserDataService,private  _global: Global) {

		this.toasterService = toasterService;
	}
	ngOnInit(): void {
	
		this.GetBranding();
	}
	//popToast() {
		
	//	var toast = {
	//		type: 'error',
	//		title: 'Here is a Toast Title',
	//		body: '<h1>Here is a Toast Body</h1>',
	//		showCloseButton: true,
	//		//positionClass: 'toast-top-left',
	//	};
	//	this.toasterService.pop(toast);
	//	//this.toasterService.pop('error', 'Error', 'Big ol');
	//}
	GetBranding(){

		this._companyDataService.getCompany("65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c").subscribe(response => {
			
			var response = response;
			//console.log(response)
			//we use alert here because toastr container is not available until html has rendered.
			 //everywhere else toastr will be available.
			if (!response.isSuccessful) {
				alert(response.error.userHelp)
			   return
			}
			this.branding = response.data;
			//console.log(this._global)
			this._global.minToChangeTeam = this.branding.minToChangeTeam;
			
			this._appUserDataService.getLoggedInUser().subscribe(response => {
				var response = response;
				this.loggedInUser = response.data;
				this._global.loggedInUser = this.loggedInUser;
			
				if (!response.isSuccessful) {
					alert(response.error.userHelp)
					return
				}
			}, error => this.msg = <any>error);
		}, error => this.msg = <any>error); 
		if(this.msg!=null){alert(this.msg)}
	
	}

}
	
