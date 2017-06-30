"use strict";
var router_1 = require("@angular/router");
var users_component_1 = require("./components/administration/users/users.component");
var modifyUsers_component_1 = require("./components/administration/users/modifyUsers.component");
var home_component_1 = require("./components/home/home.component");
var salesGroup_Component_1 = require("./components/administration/salesGroups/salesGroup.Component");
var modifySalesGroup_Component_1 = require("./components/administration/salesGroups/modifySalesGroup.Component");
var loginMsg_component_1 = require("./components/administration/loginMessages/loginMsg.component");
var modifyLoginMsg_component_1 = require("./components/administration/loginMessages/modifyLoginMsg.component");
var appRoutes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: home_component_1.HomeComponent },
    { path: 'user', component: users_component_1.UserComponent },
    { path: 'modifyUser/:userId', component: modifyUsers_component_1.modifyUsersComponent },
    { path: 'modifyUser', component: modifyUsers_component_1.modifyUsersComponent },
    { path: 'salesgroups', component: salesGroup_Component_1.SalesGroupComponent },
    { path: 'modifySalesgroup', component: modifySalesGroup_Component_1.ModifySalesGroupComponent },
    { path: 'loginMsg', component: loginMsg_component_1.LoginMsgComponent },
    { path: 'modifyLoginMsg/:messageId', component: modifyLoginMsg_component_1.ModifyLoginMsgComponent }
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routing.js.map