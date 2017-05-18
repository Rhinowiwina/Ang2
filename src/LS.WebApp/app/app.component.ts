import { Component } from "@angular/core"
import { HeaderComponent } from "./common/header"
import { BrandingComponent } from './common/branding'

@Component({
	
	selector: "my-app",	
	template: `	
			<app-branding></app-branding>
		      <app-header></app-header>
            <div class='container'>
                <router-outlet></router-outlet>
            </div>
			`,

	})

export class AppComponent {
	
	constructor() {
		
	}
	theval: string = 'red';
	custstyle: string ='.navbar {background - color:'+ this.theval +'}'
};
	
}