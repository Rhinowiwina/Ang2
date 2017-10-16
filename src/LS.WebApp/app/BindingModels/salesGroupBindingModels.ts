import { Company } from './companyBindingModels';
import { UserView, GroupManager } from './userBindingModels';
import { SalesTeam } from './salesTeamBindingModels';
export class GroupView {
    id: string;
    name: string;
    level: string;
}
export class SalesGroup {
    id: string;
    name: string;
    level: number;
    parentGroupName: string;
}
export class GroupModified {
    parentGroupOptions: Array<SalesGroup>;
    level: number;
    parentSalesGroupLabel: string
    parentGroupName: string;
    parentSalesGroupId: string;
    id: string;
    managers: Array<GroupManager>;
    name: string;
    isDeleted: boolean   
    managerOptions: Array<GroupManager>;   
}
export class Level1SalesGroup {
    id: string;
    name: string; 
    companyId: string;
    company: Company;
    managers: Array<UserView>;
    childSalesGroups: Array<Level2SalesGroup>;
    isDeleted: boolean
    createdByUserId: string;
    modifiedByUserId: string;
    createdByUser:UserView;
    dateCreated: Date;
    dateModified: Date;

}
export class Level2SalesGroup {
    id: string;
    name: string;
    companyId: string;
    company: Company;
    managers: Array<UserView>;
    childSalesGroups: Array<Level3SalesGroup>;
    sarentSalesGroupId: string;
    sarentSalesGroup: Level1SalesGroup;
    isDeleted: boolean
    createdByUserId: string;
    ModifiedByUserId: string;
    createdByUser: UserView;
    dateCreated: Date;
    dateModified: Date;

}
export class Level3SalesGroup {
    id: string;
    name: string;
    companyId: string;
    company: Company;
    managers: Array<UserView>;
    salesTeams: Array<SalesTeam>;
    childSalesGroups: Array<Level3SalesGroup>;
    parentSalesGroupId: string;
    parentSalesGroup: Level2SalesGroup;
    isDeleted: boolean
    createdByUserId: string;
    modifiedByUserId: string;
    createdByUser: UserView;
    dateCreated: Date;
    dateModified: Date;

}