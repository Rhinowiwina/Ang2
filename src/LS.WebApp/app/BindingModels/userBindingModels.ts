import { ApplicationRole } from './applicationRole';

export class UserView {
	id: string;
	firstName: string; 
	lastName: string;
	externalUserID: string;
	isExternalUserIDActive: boolean;
	userName: string;
	email: string;
	payPalEmail: string;
	isActive: boolean;
	additionalDataNeeded : boolean;
	roleId: string;
    role: ApplicationRole;
	permissionsLifelineNlad : boolean;
	permissionsLifelineCA : boolean;
	permissionsLifelineTX: boolean;
	permissionsAllowTpivBypass : boolean;
	permissionsAccountOrder : boolean;
    salesTeamId: string;
  
	//SalesTeamSimpleViewBindingModel SalesTeam 
	rowVersion: string;
	canBeDeleted: boolean;
}
export class EditUserView {
    id: string;
    companyId: string;
    userName: string;
    originalEmail:string
    firstName: string;
    lastName: string;
    externalUserID: string;
    isExternalUserIDActive: boolean;
    email: string;
    payPalEmail: string;
    isActive: boolean;
    originalActive: boolean;
    additionalDataNeeded: boolean;
    permissionsAllowTpivBypass: boolean;
    permissionsLifelineNlad: boolean;
    permissionsLifelineCA: boolean;
    permissionsLifelineTX: boolean;
    permissionsAccountOrder: boolean
    userCommission: number;
    originalRoleName: string;
    team: { id: string, name: string };
    rowVersion: string;
    roleId: string;
    role: ApplicationRole;
    selectedGroupId: string;
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