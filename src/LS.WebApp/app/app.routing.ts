import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoggedInUserResolve } from './Service/resolve.service';
import { UserComponent } from './components/administration/users/users.component';
import { modifyUsersComponent } from './components/administration/users/modifyUsers.component';
import { HomeComponent } from './components/home/home.component';
import { LoginMsgComponent } from './components/administration/loginMessages/loginMsg.component'
import { ModifyLoginMsgComponent } from './components/administration/loginMessages/modifyLoginMsg.component'
const appRoutes: Routes = [

	{ path: '', redirectTo: 'home', pathMatch: 'full' },
	{path: 'home', component: HomeComponent},
    { path: 'user', component: UserComponent },
    { path: 'modifyUser/:userId', component: modifyUsersComponent },
	{ path: 'loginMsg', component: LoginMsgComponent },
	{ path: 'modifyLoginMsg/:messageId', component: ModifyLoginMsgComponent }
];

export const routing: ModuleWithProviders =
    RouterModule.forRoot(appRoutes);