"use strict";
var router_1 = require("@angular/router");
var users_component_1 = require("./components/administration/users/users.component");
var home_component_1 = require("./components/home/home.component");
var appRoutes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: home_component_1.HomeComponent },
    { path: 'user', component: users_component_1.UserComponent },
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routing.js.map