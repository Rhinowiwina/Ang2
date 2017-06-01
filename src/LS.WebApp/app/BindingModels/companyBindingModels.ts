
export class Company {
	id: string;
	name: string;
	companyLogoUrl: string;
    primaryColorHex: string;
	secondaryColorHex: string;
	emailRequiredForOrder: boolean;
	dataImportFilePrefix: string;
	orderStart: string;
	orderEnd: string;
	timeZone: string;
	minToChangeTeam: number;
	doWhiteListCheck: number;
	doPromocodeCheck: number;
	maxCommission: number;
	notes: string;

}
