import { Component, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { Global } from '../../../Shared/global';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { ModalComponent } from 'ng2-bs3-modal/ng2-bs3-modal';
import { UsersdetailComponent } from '../../../components/administration/users/users-detail.component';
import { UserView } from '../../../BindingModels/userBindingModels';
import { AppUserDataService } from '../../../Service/Services';
import { Constants } from '../../../Shared/global';
//
@Component({
	template: `<div *ngIf="_global.loggedInUser && !loading" >
	<user-detail > </user-detail>
      </div>
	<img src= "/Content/img/spiffygif_30x30.gif" style= "text-align: center" [hidden] = "!loading"/>
		`,	
	styleUrls: ['../../Content/sass/siteAngular.css']
})
export class UserComponent implements OnInit {
	msg: string;
	loading: boolean = true;
	errmsg: string;
	loginmsg: {};
	hasLoadded: boolean = false;
	minToChangeTeam: number;
	unassignedManager: boolean = false;
	roles: {}
	users: {
		admins: Array<UserView>,
		level1Managers: Array<UserView>,
		level2Managers: Array<UserView>, 
		level3Managers: Array<UserView>,
		teamManagers: Array<UserView>, 
		reps: Array<UserView>
	};
	private toasterService: ToasterService;
	constructor(private _userDataService: AppUserDataService, private _global: Global, toasterService: ToasterService, private _constants: Constants) {
	alert('contr')
	this.toasterService = toasterService;

	}

	ngOnInit(): void {
		this.loading = false;
		//if (typeof this._global.loggedInUser == "undefined") {
		//	this._userDataService.getLoggedInUser().subscribe(response => {
		//		var response = response;
		//		this._global.loggedInUser = response.data;
		//		this.loading = false;
		//		if (!response.isSuccessful) {
		//			alert(response.error.userHelp)
		//			return
		//		}
		//	}, error => this.msg = <any>error);

		//} else { }

		//if (this._global.loggedInUser.role.rank > this._constants.salesTeamManagerRoleRank) {
		//	this.toasterService.pop('error', 'Permission Error', 'You are not authorized to view this page')
		//	return
		//} else {
		//	this._userDataService.getAllRoles().subscribe
		//		(response => {
		//			var response = response;
		//			if (!response.isSuccessful) {
		//				this.toasterService.pop('error', 'Error Getting Roles Messages', response.errror.userHelp);
		//				return
		//			}
		//			this.roles = response.data;
		//			this.loading = false;
		//		}, error => this.msg = <any>error)


		//}//endelse


	
	}

	
	

		//loadUsers(rank:number,index:number) {

			//this._userDataService.getAllUsersUnderLoggedInUserInTree().subscribe(response => {
			//	var response = response;
			//	if (!response.isSuccessful) {
			//		this.toasterService.pop('error', 'Error Getting Login Messages', response.errror.userHelp);
			//	}
			//	this.messages = response.data;
			//	for (let i = 0; i < this.messages.length; i++) {

			//		if (this.messages[i].msgLevel == 1) {
			//			this.criticalMsg.push(this.messages[i])

			//		}
			//	}
			//	//alert(this._global.criticalMsgRead)
			//	if (this.criticalMsg.length > 0) {//&& !this._global.criticalMsgRead
			//		//this.modal.open('lg')
			//		this.showModalSpinner = false;
			//	}
			//}, error => this.msg = <any>error);

		//}
		//setCriticalMsgRead() {
		//	this.modal.close()
		//	//this._global.criticalMsgRead = true;

		//}
	
}