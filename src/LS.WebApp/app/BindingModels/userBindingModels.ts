import { ApplicationRole } from './applicationRole';

export class UserView {
	Id: string;
	FirstName: string; 
	LastName: string;
	ExternalUserID: string;
	IsExternalUserIDActive: boolean;
	UserName: string;
	Email: string;
	PayPalEmail: string;
	IsActive: boolean;
	AdditionalDataNeeded : boolean;
	RoleId: string;
	//RoleSimpleViewBindingModel Role 
	PermissionsLifelineNlad : boolean;
	PermissionsLifelineCA : boolean;
	PermissionsLifelineTX: boolean;
	PermissionsAllowTpivBypass : boolean;
	PermissionsAccountOrder : boolean;
	SalesTeamId: string;
	//SalesTeamSimpleViewBindingModel SalesTeam 
	RowVersion: string;
	CanBeDeleted: boolean;
}
export class LoggedInUser {
	Id: string;
	Username: string;
	FirstName: string;
	LastName: string;
	ExternalUserID: string;
	Company: {};
	role: ApplicationRole;
	Email: string;
	PayPalEmail: string;
	SalesTeamId: string;
	SalesTeamSigType: string;
	StaleTeamActive: boolean;
	Language: string;
	ServerEnvironment: string;

	PermissionsLifelineNlad: boolean;
	PermissionsLifelineCA: boolean;
	PermissionsLifelineTX: boolean;
	PermissionsAllowTpivBypass: boolean;
	PermissionsAccountOrder: boolean;


}