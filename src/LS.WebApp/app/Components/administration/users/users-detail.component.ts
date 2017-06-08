import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { Global } from '../../../Shared/global';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { ModalComponent } from 'ng2-bs3-modal/ng2-bs3-modal';
import { UserView } from '../../../BindingModels/userBindingModels';
import { AppUserDataService } from '../../../Service/Services';
import { Constants } from '../../../Shared/global';
import { Accordion, AccordionGroup,AccordionHead } from '../../../Shared/accordion';
import { GridOptions } from "ag-grid/main";
//https://www.npmjs.com/package/ng2-accordion
@Component({
	selector: 'user-detail',
	templateUrl: 'app/Components/administration/users/users-detail.html',

	styleUrls: ['../../Content/sass/siteAngular.css']
})
export class UsersdetailComponent implements OnInit {

//
//live grid
	private userGridOptions: GridOptions;
	public rowData: any[] = [];
	private columnDefs: any[];
	//	must be together in this order
	@ViewChild('reportsettings')
	reportSettingsModal: ModalComponent;
	//
	msg: string;
	loading: boolean = true;
	errmsg: string;
	loginmsg: {};
	hasLoadded: boolean = false;
	minToChangeTeam: number;
	unassignedManager: boolean = false;
	private toasterService: ToasterService;
	roles: Array<{rank:number}>;
	orgUsers: Array<UserView>=[];
	users: UserView[][];
	admins: Array<UserView> = [];
	level1Managers: Array<UserView> = [];
	level2Managers: Array<UserView> = [];
	level3Managers: Array<UserView> = [];
	teamManagers: Array<UserView> = [];
	reps: Array<UserView> = [];
	loadingUsers:Array<boolean>=[];
	erroredRoles:Array<{}>=[];
	
	constructor(private _userDataService: AppUserDataService, private _global: Global, toasterService: ToasterService, private _constants: Constants) {
	//test
		this.userGridOptions = <GridOptions>{};
	
	this.createCols()

	//
	
		this.toasterService = toasterService;
		this.users = [];
		this.users.push(this.admins)
		this.users.push(this.level1Managers)
		this.users.push(this.level2Managers)
		this.users.push(this.level3Managers)
		this.users.push(this.teamManagers)
		this.users.push(this.reps)
		for (let i = 0; i < 6; i++) {
			this.loadingUsers.push(false);
			this.erroredRoles.push({})
		}
		
	
	}

	ngOnInit(): void {
		
        this.minToChangeTeam = this._global.minToChangeTeam;
	
		if (this._global.loggedInUser.role.rank > this._constants.salesTeamManagerRoleRank) {
			this.toasterService.pop('error', 'Permission Error', 'You are not authorized to view this page')
			return
		} else {
			this._userDataService.getAllRoles().subscribe
				(response => {
					var response = response;
					if (!response.isSuccessful) {
						this.toasterService.pop('error', 'Error Getting Roles', response.errror.userHelp);
						return
					}
					this.roles = response.data;
					//filter roles the user does not have access to.
					this.roles = this.roles.filter(role => role.rank > this._global.loggedInUser.role.rank)
			
					this.hasLoadded = true;
				}, error => this.msg = <any>error)

		}//endelse
	
	}
	
	createCols() {
		this.columnDefs= [
			{
				headerName: "Name", headerTooltip: "Active", field: "fullName", minWidth: 70, width: 185,
				cellRenderer: function (params:any) {
					if (params.data) {			
						return '<span><a (click)="modifyUser('+params.data.id+')" class="text-link">'+params.data.fullName+'</a></span>';
					} 
				} },
			{
				headerName: "Username", field: "userName", headerTooltip: "Username", minWidth: 85, width: 200,  },
			{ headerName: "Promo Code", field: "externalUserID", headerTooltip: "External User ID", minWidth: 85, width: 125, },
			{ headerName: "Team Name", field: "team", headerTooltip: "Team Name", minWidth: 85, width: 200, },
			{ headerName: "Agent ID", field: "externalDisplayName", headerTooltip: "Agent ID", minWidth: 80, width: 100, },
			{ headerName: "Active", field: "isActive", headerTooltip: "Active", minWidth: 65, width: 70, }
		];



	}

	//
	loadUsers(rank: number, index: number) {
		let self = this; //not sure why but must put key work this in variable to use in function call
		
			this.loadingUsers[index] = true;
			this._userDataService.getAllUsersUnderLoggedInUserInTree(this._global.loggedInUser.id,rank).subscribe(response => {
				var response = response;
				if (!response.isSuccessful) {
					this.loadingUsers[index] = false;
					this.toasterService.pop('error', 'Error Getting Users', response.errror.userHelp);
				}
			
				this.users[index] = response.data;
			
			this.loadingUsers[index] = false;
				this.userGridOptions = {
					columnDefs:this.columnDefs,
					//angularCompileRows: true, not used in version2
					rowData: this.users[index],
					enableColResize: true,
					enableSorting: true,
					enableFilter: true,
					rowHeight:22,
					suppressRowClickSelection: true,					
					onGridReady: function (event) {
					 self.sizeToFit('u');
					 self.autoSizeAll("u");
					},
					getRowStyle: function (params:any) {
						if (params.node.floating) {
							return { 'font-weight': 'bold' }
						}
					},
					//isExternalFilterPresent: isExternalFilterPresent,
					//doesExternalFilterPass: doesExternalFilterPass
				};	
				
			}, error => this.msg = <any>error);

	}

//	function isExternalFilterPresent() {
//	return $scope.showInactiveUsers != true;
//}

//function doesExternalFilterPass(node) {
//	if ($scope.showInactiveUsers) { //This part never actually runs because "false" is returned from isExternalFilterPresent function, which means this function is not ran at all (and all rows are shown)
//		return node.data.isActive == true || node.data.isActive == false;
//	} else {
//		return node.data.isActive == true;
//	}
//	 }

	sizeToFit(form:string) {

	if (form == 'u') {
		this.userGridOptions.api.sizeColumnsToFit();
	} else {
		this.userGridOptions.api.sizeColumnsToFit();
	}
}
	autoSizeAll(form: string) {

		if (form == 'u') {
		
		var allColumnIds :string[]=[];
		this.columnDefs.forEach(function (columnDef) {
			allColumnIds.push(columnDef.field);
		});
	this.userGridOptions.columnApi.autoSizeColumns(allColumnIds);
	//} else {
	//	var allColumnIds:string[];
	//this.searchColumnDefs.forEach(function (columnDef) {
	//		allColumnIds.push(columnDef.field);
	//	});
	//	this.gridOptions.columnApi.autoSizeColumns(allColumnIds);
	}
}
onBtExport(form: string) {
	var exportName: string;
	if (form == 'u') {
		var params = {
			allColumns: true,
			fileName: exportName,
		};
		this.userGridOptions.api.exportDataAsCsv(params);

//	} else {
//		//var params = {
//		//	allColumns: true,
//		//	fileName: $scope.exportName,
//		//};
//		//$scope.gridOptions.api.exportDataAsCsv(params);
	}
}

// externalFilterChanged(newValue) {
//	$scope.userGridOptions.api.onFilterChanged();
//}

//filterUsers(index) {
//	var newUserArray = [];
//	$scope.orgUsers[index].forEach(function (user) {
//		if ($scope.showInactiveUsers) {
//			newUserArray.push(user);
//		} else {
//			if (user.isActive) {
//				newUserArray.push(user);
//			}
//		}
//	});

//	if (typeof $scope.userGridOptions.api !== 'undefined') {
//		$scope.userGridOptions.api.setRowData(newUserArray);
//	} else {
//		$scope.userGridOptions.rowData = newUserArray;
//	}
//}

closeModal(vmodal: string) {
	if (vmodal == "reportModal") {
		this.reportSettingsModal.close()
	}


}
openModal(vmodal: string) {

		if(vmodal == "reportModal") {
			this.reportSettingsModal.open('lg')
		}
	}
}