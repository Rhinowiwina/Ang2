import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoggedInUserResolve } from './Service/resolve.service';
import { UserComponent } from './components/administration/users/users.component';
import { modifyUsersComponent } from './components/administration/users/modifyUsers.component';
import { HeaderComponent } from './common/header';
import { HomeComponent } from './components/home/home.component';
import { SalesGroupComponent } from './components/administration/salesGroups/salesGroup.Component';
import { ModifySalesGroupComponent } from './components/administration/salesGroups/modifySalesGroup.Component';
import { LoginMsgComponent } from './components/administration/loginMessages/loginMsg.component'
import { ModifyLoginMsgComponent } from './components/administration/loginMessages/modifyLoginMsg.component'
const appRoutes: Routes = [

	{ path: '', redirectTo: 'home', pathMatch: 'full' },
	{ path: 'home', component: HomeComponent},
    { path: 'user', component: UserComponent },
    { path: 'modifyUser/:userId', component: modifyUsersComponent },
    { path: 'modifyUser', component: modifyUsersComponent },
    { path: 'salesgroups', component: SalesGroupComponent },
    { path: 'modifySalesgroup', component: ModifySalesGroupComponent },
	{ path: 'loginMsg', component: LoginMsgComponent },
    { path: 'modifyLoginMsg/:messageId', component: ModifyLoginMsgComponent },
    //{ path: 'logout/:userid', component: HeaderComponent },
];

export const routing: ModuleWithProviders =
    RouterModule.forRoot(appRoutes);