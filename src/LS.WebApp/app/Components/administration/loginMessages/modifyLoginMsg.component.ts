import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, OnDestroy, Inject, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute,Router} from '@angular/router';
import { DatePipe } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser'

import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { Global } from '../../../Shared/global';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { message } from '../../../BindingModels/messageBindingModels';
import { MessageDataService } from '../../../Service/Services'

import { GridOptions } from "ag-grid/main";
import { LoggedInUser  } from '../../../BindingModels/userBindingModels';
import { AppUserDataService } from '../../../Service/Services';
import { Constants } from '../../../Shared/global';
import { YesNo } from '../../../Shared/filters';
import { DatepickerModule } from 'ngx-bootstrap';
//
@Component({

    templateUrl: '../../../app/components/administration/loginMessages/modifyLoginMsg.html',
    styleUrls: ['../../Content/sass/siteAngular.css']
})

export class ModifyLoginMsgComponent implements OnInit, OnDestroy {

    private toasterService: ToasterService;
    sub: any;
    msg: string;
    loading: boolean = true;
    errmsg: string;
    hasLoadded: boolean = false;
    message: message;
    private model: Object = { date: { year: 0, month: 0, day: 0 } };
    messageId: string;
    createOrModify: number;
    levels = [{ id: 1, name: 'Critical' }, { id: 2, name: 'Important' }, { id: 3, name: 'Informational' }]
    constructor(private router: Router, private _messageDataService: MessageDataService, private _global: Global,  toasterService: ToasterService, private _constants: Constants, private datePipe: DatePipe, private route: ActivatedRoute) {
        this.toasterService = toasterService;
    }

    ngOnInit(): void {

        this.sub = this.route.params.subscribe(params => {
            this.messageId = params['messageId']
            if (this.messageId) {
                this.createOrModify = this._constants.modify
                this.getMessageToEdit(this.messageId);
            } else {
                this.createNewMsg();
            }

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
                this.loading = false
            }

            this.message = response.data;


            this.loading = false;
        }, error => this.msg = <any>error);

    }
    createNewMsg() {
        this.message = new message();
        this.createOrModify = this._constants.create
        this.message.active = true;
        this.message.id = "";
        this.message.title = "";
        this.message.msg = "";
        this.message.beginDate = null;
        this.message.expirationDate = null;
        this.message.msgLevel = null
        this.loading = false;
    }
    submitMessage( form: any) {
        
        if (form.valid) {
          
            this.loading = true;
            this._messageDataService.submittMessageForAddOrEdit(this.createOrModify, this.message).subscribe(response => {
                var response = response;
                if (!response.isSuccessful) {
                    this.loading = false;
                    this.toasterService.pop('error', 'Error Updating Login Message.', response.errror.userHelp);
                    this.loading = false
                }
                if (this.createOrModify = this._constants.create) {
                    var returnMsg = "Successfully added message."
                } else {
                  var returnMsg ="Successfully edited message."
                }
               
                this.toasterService.pop('success',returnMsg);
                this.loading = false;
                this.router.navigate(["loginMsg"])
            }, error => this.msg = <any>error);



    
      }
    }
}