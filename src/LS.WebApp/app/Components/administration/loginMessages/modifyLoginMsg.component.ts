import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, OnDestroy, Inject, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser'

import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { Global } from '../../../Shared/global';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { ModalComponent } from 'ng2-bs3-modal/ng2-bs3-modal';
import { message } from '../../../BindingModels/messageBindingModels';
import { MessageDataService } from '../../../Service/Services'

import { GridOptions } from "ag-grid/main";
import { LoggedInUser  } from '../../../BindingModels/userBindingModels';
import { AppUserDataService } from '../../../Service/Services';
import { Constants } from '../../../Shared/global';
import { YesNo } from '../../../Shared/filters';
import { IMyDpOptions } from 'mydatepicker';
//import { DatepickerModule } from 'angular2-material-datepicker'
//
@Component({
	
	templateUrl:'../../../app/components/administration/loginMessages/modifyLoginMsg.html',
	styleUrls: ['../../Content/sass/siteAngular.css']
})

export class ModifyLoginMsgComponent implements OnInit,OnDestroy {
	private myDatePickerOptions: IMyDpOptions = {
		dateFormat: 'mm/dd/yyyy',
		markCurrentDay: true,
		showClearDateBtn:false
	}

	private toasterService: ToasterService;
	sub: any;
	msg: string;
	loading: boolean = true;
	errmsg: string;
	hasLoadded: boolean = false;
	message: message;
	private model: Object = { date: { year:0 , month: 0, day: 0 } };
	messageId: string;
	createOrModify: number;
	levels = [{ id: 1, name: 'Critical' }, { id: 2, name: 'Important' }, { id: 3, name: 'Informational' }]
	constructor(private _messageDataService: MessageDataService, private _global: Global, toasterService: ToasterService, private _constants: Constants, private datePipe: DatePipe, private route: ActivatedRoute) {
		this.toasterService = toasterService;	
	}

	ngOnInit(): void {

		this.sub = this.route.params.subscribe(params => {
		  this.messageId = params['messageId']	
			this.getMessageToEdit(this.messageId);
		})
	}
	ngOnDestroy() {
		this.sub.unsubscribe();
	}
	getMessageToEdit(messageId: string) {		
		this.loading = true;
	
		this._messageDataService.getMsgToEdit(messageId).subscribe(response => {
			var response = response;
			if (!response.isSuccessful) {
				this.loading = false;
				this.toasterService.pop('error', 'Error Getting Login Message.', response.errror.userHelp);
				this.loading=false
			}

			this.message = response.data;
			//this.model = { date: { year: 2018, month: 10, day: 9 } };
		console.log(this.message)
			this.loading = false;
		}, error => this.msg = <any>error);

	}


}