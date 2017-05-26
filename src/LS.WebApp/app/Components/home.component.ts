
import { Component,ViewChild ,ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import {ModalComponent} from 'ng2-bs3-modal/ng2-bs3-modal';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { EmitterService } from '../Service/emitter.service';
import { MessageDataService } from '../Service/Services';
import { Global } from '../Shared/global';

import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router, ActivatedRoute, Params } from '@angular/router';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
@Component({
templateUrl: "app/Components/home.component.html",
providers:[MessageDataService],
	styleUrls:['../../Content/sass/siteAngular.css']
})

export class HomeComponent implements OnInit{
	private toasterService: ToasterService;

	@ViewChild('modal')
	modal: ModalComponent;
	msg: string;
	loginmsg: {};	
	messages:Array<message>;
	criticalMsg: Array<message> = [];
	showModalSpinner: boolean=true
	constructor(private router: Router,toasterService: ToasterService, private _global: Global, private _messageDataService: MessageDataService, ) {
		this.toasterService = toasterService;
		
	}
	
	ngOnInit(): void {
	
		this.getMessages();
		this._global.criticalMsgRead = false;
	}
	
	getMessages() {
		
		this._messageDataService.getActiveMessages().subscribe(response => {
			var response = response;
			if (!response.isSuccessful){
				this.toasterService.pop('error', 'Error Getting Login Messages', response.errror.userHelp);
			}
			this.messages = response.data;
			for (let i = 0; i < this.messages.length; i++) {
				
				if (this.messages[i].msgLevel == 1) {
					this.criticalMsg.push(this.messages[i])
				 
				}
			}
			if (this.criticalMsg.length > 0 && !this._global.criticalMsgRead) {
				this.modal.open('lg')
				this.showModalSpinner = false;
			}
		}, error => this.msg = <any>error);

	}
}
class message {
	active: boolean;
	beginDate: Date;
	expirationDate: Date;
	id: string;
	msg: string;
	msgLevel: Number; 
	title: string;

}