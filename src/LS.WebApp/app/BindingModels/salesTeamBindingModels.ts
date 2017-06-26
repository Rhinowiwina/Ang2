import { Company } from './companyBindingModels';
import { Level3SalesGroup } from './groupBindingModels';
import { UserView } from './userBindingModels';
export class SalesTeam {
	id: string;
	name: string; 
	companyId: string;
    company:Company;
    externalPrimaryId: boolean;
    externalDisplayName: string;
    address1: string;
    address2: string;
    city: string;
    state: string;
    zip: string;
    phone: string;
    taxId: string;
    payPalEmail: string;
    level3SalesGroupId: string;
    sigType: string;
    level3SalesGroup:Level3SalesGroup;
    createdByUserId: string;
    modifiedByUserId: string;
    users:Array<UserView>
    isActive: boolean;
    isDeleted: boolean;
    dateCreated: Date;
    dateModified: Date;
}
export class DisplaySalesTeam {
    id: string;
    name: string;
    externalDisplayName: string;
    isActive: boolean;
    displayText: string;
}

