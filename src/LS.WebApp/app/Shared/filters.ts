import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
	name: 'YesNo',
	pure: false
})
export class YesNo implements PipeTransform {
	transform(data:boolean, filter: Object): string{
	  

		if (typeof data=='undefined'|| data ==null) {
			return "";
		 }
     
		if (data == true) { return 'Yes' }
		else {  return 'No' }
		
	
		
	}
}

@Pipe({
	name: 'msgLevel',
	pure: false
})
export class MsgLevel implements PipeTransform {
	transform(input:number, filter: Object): string {
		if (input == 1) {
			return 'Warning'
		} else if (input == 2) {
			return 'Important'

		} else if (input == 3) {
			return 'Informational'
		}


	}
}