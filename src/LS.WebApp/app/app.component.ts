import { Component, OnInit, ViewChild, Input, } from "@angular/core"
import { BrandingService } from './Service/branding.service';
import { Observable } from 'rxjs/Rx';
import { Global } from './Shared/global';
import { HeaderComponent } from "./common/header"
import { BrandingComponent } from './common/branding'
@Component({
	
	selector: "my-app",	
	template: `	
			<div *ngIf="branding">
		      <app-header> 
            [brandingmodel]="branding"
             test="test"
        </app-header></div>
            <div class='container'>
                <router-outlet></router-outlet>
            </div>
			`,
	
	providers: [BrandingService],

	})

export class AppComponent implements OnInit {

	branding: {test:'test2'};
	msg: string;
	constructor(private _brandingService: BrandingService) {}
	ngOnInit(): void {

		this.GetBranding();
		console.log(this.branding)
	}
	GetBranding() {

		return this._brandingService.get("api/company/getCompany?companyId=65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c")
			.subscribe(branding => {
				this.branding = branding.Data;
				
			}, error => this.msg = <any>error);

		
	}
}
	
