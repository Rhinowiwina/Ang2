import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { LoggedInUser } from '../BindingModels/userBindingModels';


@Injectable()
export class Global {
	private _loggedInUser: LoggedInUser;
	private _criticalMsgRead: boolean=false;
	private _minToChangeTeam: number

	get minToChangeTeam() {
		return this._minToChangeTeam;
	}
	set minToChangeTeam(val : number) {
		this._minToChangeTeam = val;

	}
	set loggedInUser(val: LoggedInUser) {
		this._loggedInUser = val;
	}
	get loggedInUser() {
		return this._loggedInUser;
	}
	set criticalMsgRead(val: boolean) {
		this._criticalMsgRead = val	}
	get criticalMsgRead():boolean {
	return	this._criticalMsgRead
	}
}
@Injectable()
export class Constants {
	get superadministratorRoleName() {
		return  "Super Administrator"
	}
	get superadministratorRoleRank() {
		return 0
	}
	get administratorRoleName() {
		return "Administrator"
	}
	get administratorRoleRank() {
		return 1
	}
	get level1ManagerRoleName() {
		return "Level 1 Manager"
	}
	get level1ManagerRoleRank() {
		return 2
	}
	get level2ManagerRoleName() {
		return "Level 2 Manager"
	}
	get level2ManagerRoleRank() {
		return 3
	}
	get level3ManagerRoleName() {
		return "Level 3 Manager"
	}
	get level3ManagerRoleRank() {
		return 4
	}
	get salesTeamManagerRoleName() {
		return "Sales Team Manager"
	}
	get salesTeamManagerRoleRank() {
		return 5
	}
	get salesRepRoleName() {
		return "Sales Rep"
	}
	get salesRepRoleRank() {
		return 6
	}
}