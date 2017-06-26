import { UserView } from './userBindingModels';
import { Company } from './companyBindingModels';
import { SalesTeam } from './salesTeamBindingModels';
export class GroupView{
    id: string;
    name: string;
    level: string;
}
export class Level1SalesGroup {
	id: string;
	name: string; 
    companyId: string;
    company: Company;
    managers: Array<UserView>;
    childSalesGroups: Array<Level2SalesGroup>;
    isDeleted: boolean;
    createdByUserId: string;
    modifiedByUserId: string;
    createdByUser: UserView;
	dateCreated:Date;
    dateModified: Date;
    createdBy: UserView;
}
export class Level2SalesGroup {
    id: string;
    name: string;
    companyId: string;
    company: Company;
    managers: Array<UserView>;
    childSalesGroups: Array<Level3SalesGroup>;
    parentSalesGroupId: string;
    parentSalesGroup: Level1SalesGroup;
    isDeleted: boolean;
    createdByUserId: string;
    modifiedByUserId: string;
    createdByUser: UserView;
    dateCreated: Date;
    dateModified: Date;
    createdBy: UserView;
}
export class Level3SalesGroup {
    id: string;
    name: string;
    companyId: string;
    company: Company;
    managers: Array<UserView>;
    parentSalesGroupId: string;
    parentSalesGroup: Level2SalesGroup;
    salesTeams: Array<SalesTeam>;
    isDeleted: boolean;
    createdByUserId: string;
    modifiedByUserId: string;
    createdByUser: UserView;
    dateCreated: Date;
    dateModified: Date;
    createdBy: UserView;
}
