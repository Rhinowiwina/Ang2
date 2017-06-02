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
	id: string;
	username: string;
	firstName: string;
	lastName: string;
	externalUserID: string;
	company: {};
	role: ApplicationRole;
	email: string;
	payPalEmail: string;
	salesTeamId: string;
	salesTeamSigType: string;
	staleTeamActive: boolean;
	language: string;
	serverEnvironment: string;

	permissionsLifelineNlad: boolean;
	permissionsLifelineCA: boolean;
	permissionsLifelineTX: boolean;
	permissionsAllowTpivBypass: boolean;
	permissionsAccountOrder: boolean;


}