import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
@Injectable()
export class Global {
	private loggedInUser: string;
	private _criticalMsgRead: boolean=false;
	

	set criticalMsgRead(val: boolean) {
		this._criticalMsgRead = val	}
	get criticalMsgRead():boolean {
	return	this._criticalMsgRead
	}
}