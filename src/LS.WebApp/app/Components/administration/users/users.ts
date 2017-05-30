import { Component, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { ModalComponent } from 'ng2-bs3-modal/ng2-bs3-modal';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { Global } from '../../../Shared/global';
import { UserView } from '../../../BindingModels/userBindingModels';
import { AppUserDataService } from '../../../Service/Services';
@Component({
	templateUrl: "app/Components/admin/users/users.html",
	styleUrls: ['../../Content/sass/siteAngular.css']
})
export class UserComponent implements OnInit {
	errmsg: string;
	loginmsg: {};
	users: Array<UserView>;
	//criticalMsg: Array<message> = [];
	showSpinner: boolean = true
	constructor(private _userDataService: AppUserDataService, private _global: Global) {


	}

	ngOnInit(): void {
			if (this._global.loggedInUser.Role.Rank>){
				

		}
		//loadUsers(rank:string, index:number) {


		//}

		//getMessages() {

		//	this._messageDataService.getActiveMessages().subscribe(response => {
		//		var response = response;
		//		if (!response.isSuccessful) {
		//			this.toasterService.pop('error', 'Error Getting Login Messages', response.errror.userHelp);
		//		}
		//		this.messages = response.data;
		//		for (let i = 0; i < this.messages.length; i++) {

		//			if (this.messages[i].msgLevel == 1) {
		//				this.criticalMsg.push(this.messages[i])

		//			}
		//		}
		//		//alert(this._global.criticalMsgRead)
		//		if (this.criticalMsg.length > 0) {//&& !this._global.criticalMsgRead
		//			//this.modal.open('lg')
		//			this.showModalSpinner = false;
		//		}
		//	}, error => this.msg = <any>error);

		//}
		//setCriticalMsgRead() {
		//	this.modal.close()
		//	//this._global.criticalMsgRead = true;

		//}
	}
}