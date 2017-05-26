
import { Component, ViewEncapsulation, Input, Attribute, OnChanges, OnInit,Inject,NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { EmitterService } from '../Service/emitter.service';
import { MessageDataService } from '../Service/Services';
import { Global } from '../Shared/global';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router, ActivatedRoute, Params } from '@angular/router';

@Component({
templateUrl: "app/Components/home.component.html",
providers:[MessageDataService],
	styleUrls:['../../Content/sass/siteAngular.css']
})

export class HomeComponent implements OnInit{

	msg: string;
	messages: {};
	
	constructor( private _global: Global, private _messageDataService: MessageDataService, ) {
		
	}

	ngOnInit(): void {
		//this.popToast();
		this.getMessages();
		
	}
	

	


	getMessages() {
	
		this._messageDataService.getActiveMessages().subscribe(response => {
			var response = response;
			if (!response.isSuccessful){
			 //do error notification
			}
			this.messages=response.data
			console.log(response);			
		}, error => this.msg = <any>error);

	}
}