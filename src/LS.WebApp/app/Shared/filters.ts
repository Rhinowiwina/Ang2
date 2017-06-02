import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
	name: 'accordianRank',
	pure: false
})
export class AccordianRank implements PipeTransform {
	transform(roles: any[], filter: Object): any {
		console.log(roles)
	console.log(filter)
		if (!roles || !filter) {
			return roles;
		}
		// filter items array, items which match and return true will be kept, false will be filtered out
		return roles.filter(role => role.rank=2);
	}
}