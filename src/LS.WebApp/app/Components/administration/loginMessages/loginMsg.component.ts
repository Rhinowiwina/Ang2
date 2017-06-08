import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { Routes,Router, RouterModule,ActivatedRoute } from '@angular/router';
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
import { YesNo,MsgLevel } from '../../../Shared/filters';
//
@Component({
	//template: `<div *ngIf="_global.loggedInUser && !loading">
	//<loginMsg-detail> </loginMsg-detail>
 //     </div>
	//<img src= "/Content/img/spiffygif_30x30.gif" style= "text-align: center" [hidden] = "!loading"/>
	//	`,	
	templateUrl:'../../../app/components/administration/loginMessages/loginMsg.html',
	styleUrls: ['../../Content/sass/siteAngular.css']
})
export class LoginMsgComponent implements OnInit {

	private toasterService: ToasterService;
	//live grid must be together in this order
	private msgGridOptions: GridOptions;
	public rowData: any[] = [];
	private columnDefs: any[];
	//	
	msg: string;
	loading: boolean = true;
	errmsg: string;
	hasLoadded: boolean = false;
	messages: message[];
	
	constructor(private _messageDataService: MessageDataService, private _global: Global, toasterService: ToasterService, private _constants: Constants, private datePipe: DatePipe, private router: Router) {
	
		this.msgGridOptions = <GridOptions>{};
		this.toasterService = toasterService;
		
	}

	ngOnInit(): void {
	
		let self = this;
		let filter = new YesNo();
		let msgFilter = new MsgLevel();
		this.columnDefs = [
			{
				headerName: "Title", headerTooltip: "Title", field: "id", minWidth: 70, width: 185,
				cellRenderer: function (params: any) {
				
					var eSpan = document.createElement('div');
				
					eSpan.innerHTML = '<span> <a class="text-link" > ' + params.data.title.substring(0, 30) + ' </a></span >';
					eSpan.addEventListener('click', function () {
				
						self.router.navigate(['modifyLoginMsg',params.data.id]);
					});
					return eSpan;
				}
					
			},
			{
				headerName: "Message", field: "msg", headerTooltip: "Message", minWidth: 85, width: 200,
			},
			{
				headerName: "Start Date", field: "beginDate", headerTooltip: "Start Date", minWidth: 85, width: 125,
				cellRenderer: function (params: any) {
					if (params.data) {
						return '<span>' + self.datePipe.transform(params.data.beginDate, 'MM/dd/yyyy') + '</span>';
					}
				}
			},
			{
				headerName: "End Date", field: "expirationDate", headerTooltip: "EndDate", minWidth: 85, width: 200,
				cellRenderer: function (params: any) {
					if (params.data) {
						return '<span>' + self.datePipe.transform(params.data.expirationDate, 'MM/dd/yyyy') + '</span>';
					}
				}
			},
			{
				headerName: "Importance Level", field: "msgLevel", headerTooltip: "Importance Level", minWidth: 80, width: 100,
				cellRenderer: function (params: any) {
					if (params.data) {
						return '<span>' + msgFilter.transform(params.data.msgLevel, null) + '</span>';
					}
				}

			},
			{
				headerName: "Active", field: "active", headerTooltip: "Active", minWidth: 65, width: 70,
				cellRenderer: function (params: any) {
					if (params.data) {
						return '<span>' + filter.transform(params.data.active, null) + '</span>';
					}
				}
			}
		];
		this.getAllMessages();
	}
	getAllMessages() {
		let self = this; //not sure why but must put key work this in variable to use in function call
		this.loading = true;
		this._messageDataService.getAllMessages().subscribe(response => {
			var response = response;
			if (!response.isSuccessful) {
				this.loading = false;
				this.toasterService.pop('error', 'Error Getting Login Messages.', response.error.userHelp);
			}

			this.messages = response.data;


			this.loading = false;
			this.msgGridOptions = {
				columnDefs: this.columnDefs,
				//angularCompileRows: true, not used in version2
				rowData: this.messages,
				enableColResize: true,
				enableSorting: true,
				enableFilter: true,
				rowHeight: 22,
				suppressRowClickSelection: true,
				onGridReady: function (event) {
					self.sizeToFit();
					self.autoSizeAll();
				},
				getRowStyle: function (params: any) {
					if (params.node.floating) {
						return { 'font-weight': 'bold' }
					}
				},

			};

		}, error => this.msg = <any>error);

	}


	sizeToFit() {
		this.msgGridOptions.api.sizeColumnsToFit();
	}
	autoSizeAll() {

		var allColumnIds: string[] = [];
		this.columnDefs.forEach(function (columnDef) {
			allColumnIds.push(columnDef.field);
		});
		this.msgGridOptions.columnApi.autoSizeColumns(allColumnIds);

	}

	onBtExport() {
		var exportName: string;

		var params = {
			allColumns: true,
			fileName: exportName,
		}
		this.msgGridOptions.api.exportDataAsCsv(params);

	}
	modifyMessage(messageId: string) {
	alert('here')
		this.router.navigate(['modifyLoginMsg', messageId]);
	}

}