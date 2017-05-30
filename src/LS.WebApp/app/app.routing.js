"use strict";
var router_1 = require("@angular/router");
//import { UserComponent } from './components/user.component';
var home_component_1 = require("./components/home/home.component");
var appRoutes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: home_component_1.HomeComponent },
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routing.js.map