import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { LoggedInUser } from '../BindingModels/userBindingModels';


@Injectable()
export class Global {
	private _loggedInUser: LoggedInUser;
	private _criticalMsgRead: boolean=false;
	
	set loggedInUser(val:LoggedInUser) {

	}
	get loggedInUser() {
		return this._loggedInUser
	}
	set criticalMsgRead(val: boolean) {
		this._criticalMsgRead = val	}
	get criticalMsgRead():boolean {
	return	this._criticalMsgRead
	}
}