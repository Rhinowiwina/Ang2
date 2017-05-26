"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
//Toast Selector is put here and accessible by all components. Default config is set in toastr-config.js
//https://github.com/Stabzs/Angular2-Toaster/blob/master/README.md
//error: 'icon-error',
//info: 'icon-info',
//wait: 'icon-wait',
//success: 'icon-success',
//warning: 'icon-warning'
var core_1 = require("@angular/core");
var Services_1 = require("./Service/Services");
var angular2_toaster_1 = require("angular2-toaster");
var Services_2 = require("./Service/Services");
require("rxjs/add/operator/mergeMap");
var global_1 = require("./Shared/global");
var AppComponent = (function () {
    function AppComponent(toasterService, _companyDataService, _appUserDataService, _global) {
        this._companyDataService = _companyDataService;
        this._appUserDataService = _appUserDataService;
        this._global = _global;
        this.toasterconfig = new angular2_toaster_1.ToasterConfig({
            limit: 4,
            positionClass: 'toast-bottom-right',
            showCloseButton: true,
            tapToDismiss: false,
            bodyOutputType: angular2_toaster_1.BodyOutputType.TrustedHtml,
        });
        this.toasterService = toasterService;
    }
    AppComponent.prototype.ngOnInit = function () {
        this.GetBranding();
    };
    AppComponent.prototype.popToast = function () {
        var toast = {
            type: 'error',
            title: 'Here is a Toast Title',
            body: '<h1>Here is a Toast Body</h1>',
            showCloseButton: true,
        };
        this.toasterService.pop(toast);
        //this.toasterService.pop('error', 'Error', 'Big ol');
    };
    AppComponent.prototype.GetBranding = function () {
        var _this = this;
        this._companyDataService.getCompany("65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c").subscribe(function (response) {
            var response = response;
            console.log(response);
            //we use alert here because toastr container is not available until html has rendered.
            //everywhere else toastr will be available.
            if (!response.isSuccessful) {
                alert(response.error.userHelp);
                return;
            }
            _this.branding = response.data;
            _this._appUserDataService.getLoggedInUser().subscribe(function (response) {
                var response = response;
                _this.loggedInUser = response.data;
                if (!response.isSuccessful) {
                    alert(response.error.userHelp);
                    return;
                }
            }, function (error) { return _this.msg = error; });
        }, function (error) { return _this.msg = error; });
        if (this.msg != null) {
            alert(this.msg);
        }
    };
    return AppComponent;
}());
AppComponent = __decorate([
    core_1.Component({
        selector: "my-app",
        template: "\t <div *ngIf=\"branding && loggedInUser\" >\n\t\t      <app-header [brandingmodel]=\"branding\" [loggedInUser]=\"loggedInUser\"> \n              </app-header></div>\n     <toaster-container [toasterconfig]=\"toasterconfig\"></toaster-container>\n \n            <div class='container container-content panel panel-default'>\n                <router-outlet></router-outlet>\n            </div>\n\t\t\t",
        styles: [" \nth {\n\tbackground-color: #d9970e !important;\n}\n\n.table > thead > tr > td.info, .table > tbody > tr > td.info, .table > tfoot > tr > td.info, .table > thead > tr > th.info, .table > tbody > tr > th.info, .table > tfoot > tr > th.info, .table > thead > tr.info > td, .table > tbody > tr.info > td, .table > tfoot > tr.info > td, .table > thead > tr.info > th, .table > tbody > tr.info > th, .table > tfoot > tr.info > th {\n\tbackground-color: #CCCCCC;\n\tfont-weight: bold\n}\n\n.table-hover > tbody > tr > td.info:hover, .table-hover > tbody > tr > th.info:hover, .table-hover > tbody > tr.info:hover > td, .table-hover > tbody > tr:hover > .info, .table-hover > tbody > tr.info:hover > th {\n\tbackground-color: #BBBBBB\n}\n\n.header-logo {\n\tdisplay: block;\n\theight: 3em;\n\t\n\tbackground-repeat: no-repeat;\n\tbackground-size: contain;\n\tmargin: .5em 0\n}\n.header-nav{\n\tbackground-color:#fff !important\n}\na {\n\tcolor: #d9970e\n}\n\n.text-link {\n\tcolor: #d9970e !important;\n}\n\n.admin-link, .admin-link:hover {\n\tcolor: #d9970e !important;\n}\n\n.header-welcome > span > a {\n\tcolor: #d9970e !important;\n}\n\n.bg-primary {\n\tbackground-color: #888888;\n\tborder: 1px solid #666666;\n}\n\n.bg-info {\n\tbackground-color: #FAFAFA;\n\tborder: 1px solid #DDDDDD;\n}\n\n.padded {\n\tpadding: 1%;\n}\n\n.bottomMargin {\n\tmargin-bottom: 3%\n}\n\n.panel-heading {\n\tbackground-color: #7b7b7b !important;\n}\n\n.panel-heading-teamuser {\n\tpadding: 10px 0px;\n\tmargin: 0px 0px;\n\tborder-radius: 5px;\n\tbackground-color: #d9970e;\n}\n\n.active {\n\tbackground-color: #7b7b7b !important;\n}\n\n.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .open > a:hover, .navbar-default .navbar-nav > .open > a:focus {\n\tbackground-color: #d9970e;\n}\n\n.header-menu {\n\theight: auto;\n\tbackground-color: #d9970e !important;\n}\n\n/*Global overrides to bootstrap menu*/\n.navbar-default {\n\tbackground: none;\n\tbox-shadow: none;\n}\n\n.navbar {\n\tborder: 0px;\n\tmargin-bottom: 0px\n}\n\n.navbar-collapse {\n\t//padding: 0px;\n}\n\n.navbar-default .navbar-nav > li > a {\n\tcolor: #F3F3F3\n}\n\n.navbar-brand, .navbar-nav > li > a {\n\ttext-shadow: 0 1px 0 rgba(0,0,0,.25)\n}\n\n.navbar-default .navbar-nav > li > a:hover, .navbar-default .navbar-nav > li > a:focus {\n\tcolor: #FFFFFF\n}\n\n.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .active > a {\n\tbackground-image: none;\n}\n\n\t.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .open > a:hover, .navbar-default .navbar-nav > .open > a:focus {\n\t\tcolor: #FFFFFF\n\t}\n\n.dropdown-menu {\n\tborder-top-width: 0px\n}\n\n\t.dropdown-menu > li > a:hover, .dropdown-menu > li > a:focus {\n\t\tbackground-image: none;\n\t}\n\n.navbar-default .navbar-toggle {\n\tbackground-color: #EEEEEE\n}\n\n\t.navbar-default .navbar-toggle:hover, .navbar-default .navbar-toggle:focus {\n\t\tbackground-color: #FAFAFA\n\t}\n\n.dropdown-header {\n\tfont-weight: bold;\n\tfont-size: 13px;\n\tcolor: #111111;\n\tpadding-left: 16px !important;\n}\n\n.navbar-collapse {\n\tborder-top-width: 0px;\n\tbox-shadow: none\n}\n\n.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .active > a {\n\tbox-shadow: none\n}\n\n@media (max-width: 767px) {\n\t.navbar-default .navbar-nav .open .dropdown-menu > li > a {\n\t\tcolor: #EEEEEE\n\t}\n\n\t\t.navbar-default .navbar-nav .open .dropdown-menu > li > a:hover, .navbar-default .navbar-nav .open .dropdown-menu > li > a:focus {\n\t\t\tcolor: #FFFFFF\n\t\t}\n\n\t.dropdown-header {\n\t\tpadding-left: 16px !important;\n\t\tcolor: #FFFFFF;\n\t}\n\n\t.collapse.navbar-collapse.in {\n\t\tbox-shadow: 0 6px 12px rgba(0,0,0,.175);\n\t}\n\n\t.dropdown-menu .divider {\n\t\tbackground-color: #888888\n\t}\n\n\t.padded {\n\t\tpadding: 1.5%;\n\t}\n\n\t.bottomMargin {\n\t\tmargin-bottom: 5%\n\t}\n}\n/*END BOOTSTRAP OVERRIDE*/\n\n.order-field-title {\n\tcolor: #000 !important;\n}\n\n.order-button-small,\n.order-button-small:hover,\n.order-button-small:focus {\n\tbackground-color: #d9970e !important;\n}\n\n.order-button,\n.order-button:hover,\n.order-button:focus {\n\tbackground-color: #d9970e !important;\n}\n\n.admin-button,\n.admin-button:hover,\n.admin-button:focus {\n\tbackground-color: #d9970e !important;\n}\n\n\n.alpha-btn {\n\tmargin: 2px;\n\tfont-weight: bold;\n\tcolor: #fff;\n\tbackground-color: #7b7b7b;\n}\n\n\t.alpha-btn:hover {\n\t\tcolor: #eee;\n\t}\n\n.searchBox {\n\tborder: 1px solid #1f2022;\n\tbackground-color: #F1F1F1;\n\tpadding: 6px;\n\tmargin: 5px;\n}\n\n.grayBox {\n\tborder: 1px solid #1f2022;\n\tbackground-color: #F1F1F1;\n\tpadding: 6px;\n\tmargin: 5px;\n}\n\n.searchBox .left {\n\tfloat: left;\n}\n\n.searchBox .order-button {\n\tmargin-top: 5px\n}\n\n.clear {\n\tclear: left\n}\n\n.ag-header-container {\n\tbackground-color: #d9970e;\n\tcolor: #fff;\n\ttext-align: left;\n}\n\n.ag-secondary .ag-header-container {\n\tbackground-color: #7b7b7b !important;\n}\n\n.ag-header-cell-resize {\n\tborder-right: 1px solid #ddd !important;\n}\n\n.ag-bootstrap .ag-root {\n\tborder: 1px solid #ddd !important;\n}\n\n.ag-cell {\n\tpadding: 4px !important;\n}\n\n\t.ag-cell:focus {\n\t\toutline: auto 5px #ffef11 !important\n\t}\n\n.ag-cell-no-focus {\n\tborder-bottom: 1px solid #ddd !important;\n\tborder-right: 1px solid #ddd !important;\n}\n\n.ag-header {\n\tbackground-color: #d9970e !important;\n}\n\n.ag-secondary .ag-header {\n\tbackground-color: #7b7b7b !important;\n}\n\n.ag-row-selected {\n\tbackground-color: #AAA !important;\n}\n\n.ag-cell-focus {\n\tborder: 1px solid #ffef11 !important\n}\n\n.ag-font-style {\n\t-webkit-user-select: auto !important\n}\n\n.ag-header-cell ag-header-cell-sortable ag-header-cell-sorted-none {\n\tcolor: #fff;\n\ttext-align: left;\n}\n\n.ag-header-icon .ag-sort-ascending-icon .ag-hidden {\n\tcolor: #fff\n}\n\n.ag-header-icon .ag-sort-descending-icon {\n\tcolor: #fff\n}\n\n.ag-cell-value {\n\tpadding: 5px\n}\n\n.ag-bootstrap .ag-header-cell-label {\n\ttext-align: left\n}\n\n.ag-row:hover {\n\tbackground-color: #fcffb6 !important\n}\n\n"],
        providers: [Services_1.CompanyDataService],
        encapsulation: core_1.ViewEncapsulation.None,
    }),
    __metadata("design:paramtypes", [angular2_toaster_1.ToasterService, Services_1.CompanyDataService, Services_2.AppUserDataService, global_1.Global])
], AppComponent);
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map