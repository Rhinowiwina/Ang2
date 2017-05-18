import { Component, OnInit, ViewChild } from '@angular/core';
import { BrandingService } from '..//Service/branding.service';
import { Observable } from 'rxjs/Rx';
import { Global } from '../Shared/global';
@Component({
	selector: 'app-branding',
	
	templateUrl: 'app/common/branding.html',
	providers:[BrandingService]
	})
export class BrandingComponent implements OnInit {
        
	branding: {};
	msg: string;
	constructor(private _brandingService: BrandingService) { }
	ngOnInit(): void {
	
		this.GetBranding();
	}
	GetBranding(): void {
		
		this._brandingService.get("api/company/getCompany?companyId=65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c")
			.subscribe(branding => {
				this.branding = branding.Data;
              console.log(this.branding);
			}, error => this.msg = <any>error);
		

	}
}