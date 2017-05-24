
import { Component, ViewEncapsulation, Input, Attribute, OnChanges, OnInit } from '@angular/core';
import { EmitterService } from '../Service/emitter.service';
import { CompanyDataService } from '../Service/Services';
import { Global } from '../Shared/global';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router, ActivatedRoute, Params } from '@angular/router';
@Component({
template:``
	//templateUrl:"home.component.html"
})

export class HomeComponent implements OnInit{
	branding: {};
	msg: string;
	constructor(private _global: Global) { }
	ngOnInit(): void {

	}
	
}