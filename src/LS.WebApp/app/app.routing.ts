import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoggedInUserResolve } from './Service/resolve.service';
import { UserComponent } from './components/administration/users/users.component';
import { HomeComponent } from './components/home/home.component';

const appRoutes: Routes = [

	{ path: '', redirectTo: 'home', pathMatch: 'full' },
	{path: 'home', component: HomeComponent},
	{path: 'user', component: UserComponent},
];

export const routing: ModuleWithProviders =
    RouterModule.forRoot(appRoutes);